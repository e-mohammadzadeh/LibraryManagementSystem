using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository
{
	void Add(Author author);
	Author? FindById(Guid id, EntityFilter filter);
	Author? FindByName(string firstName, string lastName);
	IReadOnlyList<Author> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(Email email, Guid? excludeId);
	bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId);
	void Remove(Author author);
	IReadOnlyList<Author> Search(string searchItem, Func<Author, string?> selector);
	void Update(Author author, Guid? updatedBy);
}