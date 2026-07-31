using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
	public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password cannot be empty", nameof(password));
		using var hmac = new HMACSHA512();
		passwordSalt = hmac.Key;
		passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
	}


	public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password cannot be empty", nameof(password));

		if (storedHash is null || storedHash.Length == 0)
			throw new ArgumentException("Stored password hash is invalid.", nameof(storedHash));

		if (storedSalt is null || storedSalt.Length == 0)
			throw new ArgumentException("Stored password salt is invalid.", nameof(storedSalt));


		using var hmac = new HMACSHA512(storedSalt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
	}
}