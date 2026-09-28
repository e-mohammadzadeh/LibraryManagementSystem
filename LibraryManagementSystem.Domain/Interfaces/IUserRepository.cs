using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? FindById(Guid id, EntityFilter filter);
	User? FindByName(string firstName, string lastName, EntityFilter filter);
	User? FindByEmail(Email email, EntityFilter filter);
	IReadOnlyList<User> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(Email email, Guid? excludeId);
	bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId);
	void Update(User user, Guid? updatedBy);
	void Remove(User user);
	IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector);
	IReadOnlyList<User> SearchByRole(Guid roleId);
	void ReplaceRole(User user, Role newRole);
	void RenewMembership(User user, int years);
	void FlagForRemoval(User user);
	void SetPasswordHash(User user, byte[] passwordHash, byte[] passwordSalt);
	void UpdateLastLogin(User user);
	void UpdateLastLoginInLogout(User user);
}