using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? FindById(Guid id);
	User? FindByName(string firstName, string lastName);
	User? FindByEmail(string email);
	IReadOnlyList<User> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(string email, Guid? excludeId);
	bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId);
	void Update(User user);
	void Remove(User user);
	IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector);
	IReadOnlyList<User> SearchByRole(IReadOnlyList<Guid> roleId);
}