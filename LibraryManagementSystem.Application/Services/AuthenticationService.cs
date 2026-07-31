using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ICurrentUserSession _currentUserSession;


	public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher,
		ICurrentUserSession currentUserSession)
	{
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
		_currentUserSession = currentUserSession;
	}


	public ServiceResult<AuthUserDto> Login(string email, string password)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<AuthUserDto>.Fail(ValidationMessages.LoginInputRequired);

		var user = _userRepository.FindByEmail(email);
		if (user is null || !_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<AuthUserDto>.Fail(ValidationMessages.InvalidLoginInput);

		if (!user.IsActive) return ServiceResult<AuthUserDto>.Fail(ValidationMessages.InactiveAccount);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<AuthUserDto>.Fail(ValidationMessages.MembershipExpired);

		user.UpdateLastLogin();
		_userRepository.Update(user);

		var authUser = new AuthUserDto
		{
			Id = user.Id,
			FullName = $"{user.FirstName} {user.LastName}",
			Email = user.Email,
			Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList().AsReadOnly(),
			IsActive = user.IsActive,
			MembershipExpiryDate = user.MembershipExpiryDate
		};

		_currentUserSession.Login(authUser);
		return ServiceResult<AuthUserDto>.Ok(authUser, ValidationMessages.LoginSuccess);
	}


	public ServiceResult<string> Logout()
	{
		if (!_currentUserSession.IsAuthenticated) return ServiceResult<string>.Fail("No user is currently logged in.");

		var username = _currentUserSession.CurrentUser?.FullName ?? "User";
		_currentUserSession.Logout();

		return ServiceResult<string>.Ok(username, $"{username} " + ValidationMessages.LogoutSuccess);
	}


	public AuthUserDto? GetCurrentUser() { return _currentUserSession.CurrentUser; }

	public bool IsAuthenticated() { return _currentUserSession.IsAuthenticated; }

	public bool HasRole(LibraryUserRole role) { return _currentUserSession.HasRole(role); }


	public bool HasAnyRole(params LibraryUserRole[] roles)
	{
		var currentRoles = _currentUserSession.CurrentUser?.Roles;
		return currentRoles is not null && roles.Any(currentRoles.Contains);
	}
}