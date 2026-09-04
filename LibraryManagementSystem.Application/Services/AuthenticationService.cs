using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ICurrentUserSession _currentUserSession;
	private readonly IAuditLogManagementService _auditLog;


	public AuthenticationService(IUserRepository userRepository, IRoleRepository roleRepository,
		IPasswordHasher passwordHasher, ICurrentUserSession currentUserSession, IAuditLogManagementService auditLog)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_passwordHasher = passwordHasher;
		_currentUserSession = currentUserSession;
		_auditLog = auditLog;
	}


	public ServiceResult<AuthUserDto> Login(string email, string password)
	{
		email = email.Trim().ToLower();
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<AuthUserDto>.Fail(Messages.LoginInputRequired);

		var user = _userRepository.FindByEmail(email);
		if (user is null || !_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<AuthUserDto>.Fail(Messages.InvalidLoginInput);

		if (!user.IsActive) return ServiceResult<AuthUserDto>.Fail(Messages.InactiveAccount);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<AuthUserDto>.Fail(Messages.MembershipExpired);

		user.UpdateLastLogin();
		_userRepository.Update(user);
		var authUser = user.ToAuthUserDto();
		_currentUserSession.Login(authUser);
		_auditLog.Record(AuditAction.UserLoggedIn, "User", user.Id, "User loggedIn.");

		return ServiceResult<AuthUserDto>.Ok(authUser, Messages.LoginSuccess);
	}


	public ServiceResult<string> Logout()
	{
		if (!_currentUserSession.IsAuthenticated) return ServiceResult<string>.Fail(Messages.NoUserLoggedIn);

		var currentUser = _currentUserSession.CurrentUser!;
		var username = currentUser.FullName;
		var user = _userRepository.FindByEmail(currentUser.Email);

		_auditLog.Record(AuditAction.UserLoggedOut, "User", currentUser.Id, "User loggedOut.");
		user!.UpdateLastLoginInLogout();
		_currentUserSession.Logout();

		return ServiceResult<string>.Ok(username, $"\n{username} " + Messages.LogoutSuccess);
	}


	public ServiceResult<AuthUserDto> Register(CreateUserDto dto)
	{
		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.ExistsByEmail(dto.Email))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName);
		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateUserNameWarning, existingSameName.Id);

		var role = _roleRepository.FindByIds(dto.RoleIds);

		var result = _passwordHasher.CreatePasswordHash(dto.Password!);

		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			role);

		newUser.SetPasswordHash(result.Hash, result.Salt);
		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user created.");

		return warningMessage is not null
			? ServiceResult<AuthUserDto>.Warning(newUser.ToAuthUserDto(), warningMessage)
			: ServiceResult<AuthUserDto>.Ok(newUser.ToAuthUserDto(), Messages.UserRegisterationSuccessfully);
	}
}