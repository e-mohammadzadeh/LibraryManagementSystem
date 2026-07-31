using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;
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


	public ServiceResult<UserDto> Login(string email, string password)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			return ServiceResult<UserDto>.Fail("Email and password are required.");

		var user = _userRepository.FindByEmail(email);
		if (user is null)
			return ServiceResult<UserDto>.Fail("Login failed: Invalid email address.");

		if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			return ServiceResult<UserDto>.Fail("Login failed: Invalid password.");

		if (!user.IsActive)
			return ServiceResult<UserDto>.Fail("This account is inactive.");

		// Update last login date (optional)
		user.UpdateLastLogin();
		_userRepository.Update(user);

		return ServiceResult<UserDto>.Ok(MapToDto(user), "Login successful.");
	}


	public void Logout()
	{
		Logout()
			↓
		Clear CurrentUserSession
			↓
		Return to Login Menu
	}


	public void GetCurrentUser() { }

	public void IsAuthenticated(){}

}