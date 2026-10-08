using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		book.Id = Guid.CreateVersion7();
		book.CreatedAt = DateTime.UtcNow;
		book.AvailableCopies = book.TotalCopies;
		book.IsRemoved = false;
		_books.Add(book);
	}


	public Book? FindById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_books, filter).FirstOrDefault(b => b.Id == id);
	}


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter)];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.BookAuthors.Any(a => a.AuthorId == authorId))];
	}


	public IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.BookTranslators.Any(t => t.TranslatorId == translatorId))];
	}


	public IReadOnlyList<Book> GetAvailableBooks(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.AvailableCopies > 0)];
	}


	public bool ExistsByName(string name, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(name)) return false;

		return _books.Any(b =>
			b.Id != excludeId &&
			!b.IsRemoved &&
			b.Title.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(ISBN isbn, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(isbn)) return false;

		return _books.Any(b =>
			b.Id != excludeId &&
			!b.IsRemoved &&
			b.ISBN.Equals(isbn));
	}


	public void Remove(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (book.IsRemoved) return;
		book.IsRemoved = true;
		book.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];

		return
		[
			.. _books
				.Where(b => !b.IsRemoved)
				.Where(b =>
				{
					var value = selector(b);
					return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
				})
		];
	}


	public IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector)
	{
		return
		[
			.. _books
				.Where(b => !b.IsRemoved)
				.Where(b =>
				{
					var value = selector(b);
					return value >= from && value <= to;
				})
		];
	}


	public void Update(Book book, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(book);

		var tracked = _books.FirstOrDefault(b => b.Id == book.Id) ?? throw new BookNotFoundException(book.Id);

		tracked.Title = book.Title;
		tracked.ISBN = book.ISBN;
		tracked.BookAuthors = book.BookAuthors;
		tracked.BookTranslators = book.BookTranslators;
		tracked.PublishDate = book.PublishDate;
		tracked.Genre = book.Genre;
		tracked.Publisher = book.Publisher;
		tracked.OriginalLanguage = book.OriginalLanguage;
		tracked.TotalCopies = book.TotalCopies;
		tracked.AvailableCopies = book.AvailableCopies;
		tracked.Description = book.Description;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = updatedBy;
	}


	// ---------- Private helper ---------
	private static IEnumerable<Book> ApplyFilter(IEnumerable<Book> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(b => !b.IsRemoved),
			EntityFilter.Removed => source.Where(b => b.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}