using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
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
		email = email.Trim().ToLower();
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<AuthUserDto>.Fail(Messages.LoginInputRequired);

		var user = _userRepository.FindByEmail(email);
		if (user is null || !_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<AuthUserDto>.Fail(Messages.InvalidLoginInput);

		if (!user.IsActive) return ServiceResult<AuthUserDto>.Fail(Messages.InactiveAccount);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<AuthUserDto>.Fail(Messages.MembershipExpired);

		var authUser = new AuthUserDto
		{
			Id = user.Id,
			FullName = $"{user.FirstName} {user.LastName}",
			Email = user.Email,
			Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList().AsReadOnly(),
			IsActive = user.IsActive,
			MembershipExpiryDate = user.MembershipExpiryDate,
			ShouldRemove = user.ShouldRemove
		};

		user.UpdateLastLogin();
		_userRepository.Update(user);
		_currentUserSession.Login(authUser);
		return ServiceResult<AuthUserDto>.Ok(authUser, Messages.LoginSuccess);
	}


	public ServiceResult<string> Logout()
	{
		if (!_currentUserSession.IsAuthenticated) return ServiceResult<string>.Fail(Messages.NoUserLoggedIn);

		var username = _currentUserSession.CurrentUser?.FullName ?? "User";
		_currentUserSession.Logout();

		return ServiceResult<string>.Ok(username, $"{username} " + Messages.LogoutSuccess);
	}
}