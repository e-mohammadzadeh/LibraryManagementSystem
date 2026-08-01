using LibraryManagementSystem.Application.Authentication;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPasswordHasher
{
	PasswordHashResult CreatePasswordHash(string password);
	bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
}