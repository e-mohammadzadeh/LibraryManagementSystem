using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	private readonly IUserRepository _userRepository;
	private readonly IUserManagementService _userService;
	private readonly IRoleRepository _roleRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ICurrentUserSession _currentUserSession;
	private readonly IAuditLogManagementService _auditLog;


	public AuthenticationService(IUserRepository userRepository, IUserManagementService userService,
		IRoleRepository roleRepository, IPasswordHasher passwordHasher, ICurrentUserSession currentUserSession,
		IAuditLogManagementService auditLog)
	{
		_userRepository = userRepository;
		_userService = userService;
		_roleRepository = roleRepository;
		_passwordHasher = passwordHasher;
		_currentUserSession = currentUserSession;
		_auditLog = auditLog;
	}


	public ServiceResult<AuthUserDto> Login(Email email, string password)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<AuthUserDto>.Fail(Messages.LoginInputRequired);

		var user = _userRepository.GetByEmail(email, EntityFilter.Active);
		if (user is null || user.IsRemoved ||
		    !_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<AuthUserDto>.Fail(Messages.InvalidLoginInput);

		if (!user.IsActive) return ServiceResult<AuthUserDto>.Fail(Messages.InactiveAccount);

		if (user.ShouldRemove) return ServiceResult<AuthUserDto>.Fail(Messages.UserFlaggedForRemoval);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<AuthUserDto>.Fail(Messages.MembershipExpired);

		_userService.UpdateLastLogin(user.Id);

		var authUser = user.ToAuthUserDto();
		_currentUserSession.Login(authUser);
		_auditLog.Record(AuditAction.UserLoggedIn, "User", user.Id, "User logged in.");

		return ServiceResult<AuthUserDto>.Ok(authUser, Messages.LoginSuccess);
	}


	public ServiceResult<string> Logout()
	{
		if (!_currentUserSession.IsAuthenticated) return ServiceResult<string>.Fail(Messages.NoUserLoggedIn);

		var currentUser = _currentUserSession.CurrentUser!;
		var username = currentUser.FullName;
		var email = Email.Create(currentUser.Email);
		var user = _userRepository.GetByEmail(email, EntityFilter.Active);

		_auditLog.Record(AuditAction.UserLoggedOut, "User", currentUser.Id, "User logged out.");
		if (user is not null)
		{
			_userService.UpdateLastLoginInLogout(user.Id);
			_currentUserSession.Logout();
		}

		return ServiceResult<string>.Ok(username, string.Format(Messages.LogoutSuccess, username));
	}


	public ServiceResult<AuthUserDto> Register(UserDto dto)
	{
		if (_userRepository.Exists(dto.NationalCode, u => u.NationalCode))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.Exists(dto.Email, u => u.Email))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		if (_userRepository.Exists(dto.PhoneNumber, u => u.PhoneNumber))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		var role = _roleRepository.GetByName(LibraryUserRole.Member);

		var result = _passwordHasher.CreatePasswordHash(dto.Password);

		var newUser = new User
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate,
			RoleId = role!.Id,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today)
		};

		_userService.SetPasswordHash(newUser.Id, result.Hash, result.Salt);
		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user registered.");

		var authUser = newUser.ToAuthUserDto();
		_currentUserSession.Login(authUser);
		_auditLog.Record(AuditAction.UserLoggedIn, "User", newUser.Id, "User logged in after registration.");

		return ServiceResult<AuthUserDto>.Ok(authUser, Messages.UserRegistrationSuccessfully);
	}
}