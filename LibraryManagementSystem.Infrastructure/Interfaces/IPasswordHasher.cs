using LibraryManagementSystem.Infrastructure.Common;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IPasswordHasher
{
	PasswordHashResult CreatePasswordHash(string password);
	bool VerifyPassword(string password, byte[]? storedHash, byte[]? storedSalt);
}