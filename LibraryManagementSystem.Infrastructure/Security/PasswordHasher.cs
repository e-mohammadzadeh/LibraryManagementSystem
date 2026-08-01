using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
	public PasswordHashResult CreatePasswordHash(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password cannot be empty", nameof(password));
		using var hmac = new HMACSHA512();
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		return new PasswordHashResult(hash, hmac.Key);
	}


	public bool VerifyPassword(string password, byte[]? storedHash, byte[]? storedSalt)
	{
		if (string.IsNullOrWhiteSpace(password)) return false;
		if (storedHash is null || storedSalt is null) return false;
		if (storedHash.Length == 0 || storedSalt.Length == 0) return false;

		using var hmac = new HMACSHA512(storedSalt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
	}
}