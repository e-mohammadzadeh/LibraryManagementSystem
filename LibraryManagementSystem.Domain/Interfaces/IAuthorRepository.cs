using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository
{
	void Add(Author author);
	Author? FindById(Guid id);
	Author? FindByName(string firstName, string lastName);
	IReadOnlyList<Author> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(string email, Guid? excludeId);
	bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId);
	void Remove(Author author);
	IReadOnlyList<Author> Search(string searchItem, Func<Author, string?> selector);
	void Update(Author author, UpdateContributorDto dto);
}