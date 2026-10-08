namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IUserManagementService
{
	void SetPasswordHash(Guid userId, byte[] passwordHash, byte[] passwordSalt);
	void UpdateLastLogin(Guid userId);
	void UpdateLastLoginInLogout(Guid userId);
}