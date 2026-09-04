using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ICurrentUserSession _currentUserSession;
	private readonly IAuditLogManagementService _auditLog;


	public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher,
		ICurrentUserSession currentUserSession, IAuditLogManagementService auditLog)
	{
		_userRepository = userRepository;
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
		_auditLog.Record(AuditAction.UserLoggedIn, "Login", user.Id, "User loggedIn.");

		return ServiceResult<AuthUserDto>.Ok(authUser, Messages.LoginSuccess);
	}


	public ServiceResult<string> Logout()
	{
		if (!_currentUserSession.IsAuthenticated) return ServiceResult<string>.Fail(Messages.NoUserLoggedIn);

		var username = _currentUserSession.CurrentUser?.FullName ?? "User";
		var currentUserEmail = _currentUserSession.CurrentUser!.Email;
		var user = _userRepository.FindByEmail(currentUserEmail);
		_auditLog.Record(AuditAction.UserLoggedOut, "Logout", user!.Id, "User loggedOut.");
		user.UpdateLastLoginInLogout();
		_currentUserSession.Logout();

		return ServiceResult<string>.Ok(username, $"\n{username} " + Messages.LogoutSuccess);
	}
}