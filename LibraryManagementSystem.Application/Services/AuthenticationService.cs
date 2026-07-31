using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	public ServiceResult<UserDto> Login(string email, byte[] password)
	{
		var user = _userRepository.FindByEmail(email);
		if (user is null)
			return ServiceResult<UserDto>.Fail("Login failed: Invalid email address.");

		if (!IPasswordHasher.VerifyPassword(password, user.PasswordHash))
			return ServiceResult<UserDto>.Fail("Login failed: Invalid password.");

		// Update last login date (optional)
		user.RecordLogin();
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