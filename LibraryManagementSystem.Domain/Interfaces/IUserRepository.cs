using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? FindById(Guid id, EntityFilter filter);
	User? FindByName(string firstName, string lastName, EntityFilter filter);
	User? FindByEmail(Email email, EntityFilter filter);
	IReadOnlyList<User> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null);
	bool ExistsByEmail(Email email, Guid? excludeId = null);
	bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId = null);
	void Update(User user, Guid? updatedBy);
	void Remove(User user);
	IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector);
	IReadOnlyList<User> SearchByRole(Guid roleId);
}