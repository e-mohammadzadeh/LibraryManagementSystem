using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryAuthorRepository : IAuthorRepository
{
	private readonly List<Author> _authors = new();


	public void Add(Author author)
	{
		_authors.Add(author);
	}


	public Author? FindById(int id)
	{
		return _authors.FirstOrDefault(author => author.Id == id);
	}


	public Author? FindByName(string firstName, string lastName)
	{
		return _authors.FirstOrDefault(authora =>
			authora.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			authora.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Author> GetAll()
	{
		return _authors.AsReadOnly();
	}


	public bool ExistsByName(string firstName, string lastName, int excludeId = -1)
	{
		return _authors.Any(author =>
			author.Id != excludeId && author.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			author.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByNationalCode(string nationalCode, int excludeId = -1)
	{
		return _authors.Any(author => author.Id != excludeId && author.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, int excludeId = -1)
	{
		return _authors.Any(author => author.Id != excludeId && author.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1)
	{
		return _authors.Any(author => author.Id != excludeId && author.PhoneNumber.Equals(phoneNumber));
	}


	public void Remove(Author author)
	{
		_authors.Remove(author);
	}


	public IReadOnlyList<Author> Search(string searchItem, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<Author>();

		return _authors.Where(author =>
		{
			var value = selector(author);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}
}