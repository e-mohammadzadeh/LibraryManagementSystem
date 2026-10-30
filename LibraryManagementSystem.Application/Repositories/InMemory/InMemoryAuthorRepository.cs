using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryAuthorRepository : IAuthorRepository
{
	private readonly List<Author> _authors;
	public InMemoryAuthorRepository() { _authors = CreateSeedData(); }


	private static List<Author> CreateSeedData()
	{
		return
		[
			new Author
			{
				Id = Guid.CreateVersion7(),
				FirstName = "George",
				LastName = "Orwell",
				NationalCode = "1234567890",
				Email = Email.Create("orwell@example.com"),
				PhoneNumber = PhoneNumber.Create("09120000001"),
				BirthDate = new DateOnly(1903, 6, 25),
				Biography = "English novelist and essayist.",
				CreatedAt = DateTime.UtcNow.AddDays(-90),
				IsRemoved = false

			},
			new Author
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Aldous",
				LastName = "Huxley",
				NationalCode = "0987654321",
				Email = Email.Create("huxley@example.com"),
				PhoneNumber = PhoneNumber.Create("09120000002"),
				BirthDate = new DateOnly(1894, 11, 26),
				Biography = "English writer and social critic.",
				CreatedAt = DateTime.UtcNow.AddDays(-90),
				IsRemoved = false
			},
			new Author
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Ray",
				LastName = "Bradbury",
				NationalCode = "1122334455",
				Email = Email.Create("bradbury@example.com"),
				PhoneNumber = PhoneNumber.Create("09120000003"),
				BirthDate = new DateOnly(1920, 8, 22),
				Biography = "American science fiction and fantasy writer.",
				CreatedAt = DateTime.UtcNow.AddDays(-90),
				IsRemoved = false
			},
			new Author
			{
				Id = Guid.CreateVersion7(),
				FirstName = "J.K.",
				LastName = "Rowling",
				NationalCode = "0087654321",
				Email = Email.Create("rowling@example.com"),
				PhoneNumber = PhoneNumber.Create("09120000004"),
				BirthDate = new DateOnly(1965, 7, 31),
				Biography = "British author of the Harry Potter series.",
				CreatedAt = DateTime.UtcNow.AddDays(-90),
				IsRemoved = false,
			}
		];
	}


	public void Add(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		author.Id = Guid.CreateVersion7();
		author.CreatedAt = DateTime.UtcNow;
		author.IsRemoved = false;
		_authors.Add(author);
	}


	public Author? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_authors, filter).FirstOrDefault(a => a.Id == id);
	}


	public IReadOnlyList<Author> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_authors, filter)];
	}


	public bool Exists<T>(T value, Func<Author, T> selector, Guid? excludeId)
	{
		return _authors.Any(a =>
			!a.IsRemoved &&
			(excludeId is null || a.Id != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(a), value));
	}


	public void Remove(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);
		if (author.IsRemoved) return;
		author.IsRemoved = true;
		author.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(Author author)
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
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = author.UpdatedByUserId;
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