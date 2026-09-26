using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryAuthorRepository : IAuthorRepository
{
	private readonly List<Author> _authors = [];


	public void Add(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);
		_authors.Add(author);
		author.UpdatedAt = DateTime.UtcNow;
	}


	public Author? FindById(Guid id) { return _authors.FirstOrDefault(author => author.Id == id && !author.IsRemoved); }


	public Author? FindByName(string firstName, string lastName)
	{
		return _authors.FirstOrDefault(author =>
			!author.IsRemoved &&
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
			author.NationalCode.Equals(nationalCode) &&
			!author.IsRemoved &&
			(excludeId is null || author.Id != excludeId));
	}


	public bool ExistsByEmail(string email, Guid? excludeId = null)
	{
		return _authors.Any(author =>
			author.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
			!author.IsRemoved && 
			(excludeId is null || author.Id != excludeId));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId = null)
	{
		return _authors.Any(author =>
			author.PhoneNumber.Equals(phoneNumber) &&
			!author.IsRemoved &&
			(excludeId is null || author.Id != excludeId));
	}


	public void Remove(Author author)
	{
		if (author.IsRemoved) return;
		author.IsRemoved = true;
		author.UpdatedAt = DateTime.UtcNow;
	}


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


	public void Update(Author author, UpdateContributorDto dto)
	{
		author.FirstName = dto.FirstName ?? author.FirstName;
		author.LastName = dto.LastName ?? author.LastName;
		author.NationalCode = dto.NationalCode ?? author.NationalCode;
		author.Email = dto.Email ?? author.Email;
		author.PhoneNumber = dto.PhoneNumber ?? author.PhoneNumber;
		author.BirthDate = dto.BirthDate ?? author.BirthDate;
		author.Biography = dto.Biography ?? author.Biography;
		author.UpdatedAt = DateTime.UtcNow;
	}
}