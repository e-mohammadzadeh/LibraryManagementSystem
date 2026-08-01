namespace LibraryManagementSystem.Application.Authentication;

public interface IPasswordHasher
{
	PasswordHashResult CreatePasswordHash(string password);
	bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
}