using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? FindById(int id);
	User? FindByName(string firstName, string lastName);
	User? FindByEmail(string email);
	IReadOnlyList<User> GetAll(UserFilter filter);
	bool ExistsByNationalCode(string nationalCode, int excludeId = -1);
	bool ExistsByEmail(string email, int excludeId = -1);
	bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1);
	void Update(User user);
	void Remove(User user);
	IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector);
	IReadOnlyList<User> SearchByRole(IReadOnlyList<int> roleId);
}