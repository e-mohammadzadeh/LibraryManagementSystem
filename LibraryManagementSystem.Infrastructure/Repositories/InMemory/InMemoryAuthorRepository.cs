using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryAuthorRepository : IAuthorRepository
{
	private readonly List<Author> _authors = [];


	public void Add(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);
		_authors.Add(author);
	}


	public Author? FindById(Guid id) { return _authors.FirstOrDefault(author => author.Id == id); }


	public Author? FindByName(string firstName, string lastName)
	{
		return _authors.FirstOrDefault(author =>
			author.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			author.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Author> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		var query = _authors.AsEnumerable();
		switch (filter)
		{
			case EntityFilter.Active:
				query = query.Where(a => !a.IsRemoved);
				break;
			case EntityFilter.Removed:
				query = query.Where(a => a.IsRemoved);
				break;
			case EntityFilter.All:
				// No filter – include everyone
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
		}

		return [.. query];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null)
	{
		return _authors.Any(author =>
			author.NationalCode.Equals(nationalCode) && (excludeId is null || author.Id != excludeId));
	}


	public bool ExistsByEmail(string email, Guid? excludeId = null)
	{
		return _authors.Any(author =>
			author.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
			(excludeId is null || author.Id != excludeId));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId = null)
	{
		return _authors.Any(author =>
			author.PhoneNumber.Equals(phoneNumber) && (excludeId is null || author.Id != excludeId));
	}


	public void Remove(Author author) { author.IsRemoved = true; }


	public IReadOnlyList<Author> Search(string searchItem, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		return
		[
			.. _authors.Where(author =>
			{
				var value = selector(author);
				return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public void Update(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);
		var existingAuthorIndex = _authors.FindIndex(a => a.Id == author.Id);
		if (existingAuthorIndex == -1) throw new KeyNotFoundException($"Author with ID {author.Id} was not found.");
		_authors[existingAuthorIndex] = author;
	}
}