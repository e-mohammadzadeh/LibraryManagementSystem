using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository
{
	void Add(Author author);
	Author? FindById(int id);
	Author? FindByName(string firstName, string lastName);
	IReadOnlyList<Author> GetAll();
	bool ExistsByName(string firstName, string lastName, int excludeId = -1);
	bool ExistsByNationalCode(string nationalCode, int excludeId = -1);
	bool ExistsByEmail(string email, int excludeId = -1);
	bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1);
	void Remove(Author author);
	IReadOnlyList<Author> Search(string searchItem, Func<Author, string?> selector);
	int Count();
}