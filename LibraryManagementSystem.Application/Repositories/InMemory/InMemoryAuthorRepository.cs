using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryAuthorRepository : IAuthorRepository
{
	private readonly List<Author> _authors = [];


	public void Add(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		author.Id = Guid.CreateVersion7();
		author.CreatedAt = DateTime.UtcNow;
		author.IsRemoved = false;
		_authors.Add(author);
	}


	public Author? FindById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_authors, filter).FirstOrDefault(a => a.Id == id);
	}


	public Author? FindByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_authors, filter).FirstOrDefault(a =>
			!a.IsRemoved &&
			a.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			a.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Author> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_authors, filter)];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null)
	{
		return _authors.Any(a =>
			!a.IsRemoved &&
			a.NationalCode.Equals(nationalCode, StringComparison.OrdinalIgnoreCase) &&
			(excludeId is null || a.Id != excludeId));
	}


	public bool ExistsByEmail(Email email, Guid? excludeId = null)
	{
		return _authors.Any(a =>
			!a.IsRemoved &&
			a.Email.Equals(email) &&
			(excludeId is null || a.Id != excludeId));
	}


	public bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId = null)
	{
		return _authors.Any(a =>
			!a.IsRemoved &&
			a.PhoneNumber.Equals(phoneNumber) &&
			(excludeId is null || a.Id != excludeId));
	}


	public void Remove(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);
		if (author.IsRemoved) return;
		author.IsRemoved = true;
		author.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Author> Search(string searchTerm, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];

		return
		[
			.. _authors
				.Where(a => !a.IsRemoved)
				.Where(a =>
				{
					var value = selector(a);
					return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
				})
		];
	}


	public void Update(Author author, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(author);

		var tracked = _authors.FirstOrDefault(a => a.Id == author.Id) ?? throw new AuthorNotFoundException(author.Id);

		tracked.FirstName = author.FirstName;
		tracked.LastName = author.LastName;
		tracked.NationalCode = author.NationalCode;
		tracked.Email = author.Email;
		tracked.PhoneNumber = author.PhoneNumber;
		tracked.BirthDate = author.BirthDate;
		tracked.Biography = author.Biography;
		tracked.UpdatedByUserId = updatedBy;
		tracked.UpdatedAt = DateTime.UtcNow;
	}


	// ---------- Private helper ----------
	private static IEnumerable<Author> ApplyFilter(IEnumerable<Author> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(a => !a.IsRemoved),
			EntityFilter.Removed => source.Where(a => a.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}