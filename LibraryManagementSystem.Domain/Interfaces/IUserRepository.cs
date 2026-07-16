using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? FindById(int id);
	User? FindByName(string firstName, string lastName);
	IReadOnlyList<User> GetAll();
	bool ExistsByName(string firstName, string lastName, int excludeId = -1);
	bool ExistsByNationalCode(string nationalCode, int excludeId = -1);
	bool ExistsByEmail(string email, int excludeId = -1);
	bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1);
	void Update(User user);
	void Remove(User user);
	IReadOnlyList<User> Search(string searchItem, Func<User, string?> selector);
}