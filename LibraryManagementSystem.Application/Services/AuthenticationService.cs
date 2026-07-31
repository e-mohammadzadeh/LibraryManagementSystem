using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;


	public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher)
	{
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
	}


	public ServiceResult<AuthUserDto> Login(string email, string password)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<AuthUserDto>.Fail("Email and password are required.");

		var user = _userRepository.FindByEmail(email);
		if (user is null)
			return ServiceResult<AuthUserDto>.Fail("Login failed: Invalid email address.");

		if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<AuthUserDto>.Fail("Login failed: Invalid password.");

		if (!user.IsActive)
			return ServiceResult<AuthUserDto>.Fail("This account is inactive.");

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

		return ServiceResult<AuthUserDto>.Ok(authUser, "Login successful.");
	}


	public void Logout()
	{
		Logout()
			↓
		Clear CurrentUserSession
			↓
		Return to Login Menu
	}


	public void GetCurrentUser()
	{
		cure
	}

	public void IsAuthenticated(){}



}