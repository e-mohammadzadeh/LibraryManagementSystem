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

		var user = _userRepository.FindByEmail(email, EntityFilter.Active);
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
		var user = _userRepository.FindByEmail(email, EntityFilter.Active);

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
		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode!, null))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		var email = Email.Create(dto.Email!);
		if (_userRepository.ExistsByEmail(email, null))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		var phoneNumber = PhoneNumber.Create(dto.PhoneNumber!);
		if (_userRepository.ExistsByPhoneNumber(phoneNumber, null))
			return ServiceResult<AuthUserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		var existingSameName = _userRepository.FindByName(dto.FirstName!, dto.LastName!, EntityFilter.Active);
		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateUserNameWarning, existingSameName.Id);

		var role = _roleRepository.FindById(dto.RoleId!.Value);

		var result = _passwordHasher.CreatePasswordHash(dto.Password!);

		var newUser = new User
		{
			FirstName = dto.FirstName!,
			LastName = dto.LastName!,
			NationalCode = dto.NationalCode!,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = dto.BirthDate!.Value,
			RoleId = dto.RoleId!.Value,
			Role = role!,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today)
		};

		_userService.SetPasswordHash(newUser.Id, result.Hash, result.Salt);
		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user registered.");

		var authUser = newUser.ToAuthUserDto();
		_currentUserSession.Login(authUser);
		_auditLog.Record(AuditAction.UserLoggedIn, "User", newUser.Id, "User logged in after registration.");

		return warningMessage is not null
			? ServiceResult<AuthUserDto>.Warning(authUser, warningMessage)
			: ServiceResult<AuthUserDto>.Ok(authUser, Messages.UserRegistrationSuccessfully);
	}
}