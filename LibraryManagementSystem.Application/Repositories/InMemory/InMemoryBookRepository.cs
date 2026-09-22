using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);
		_books.Add(book);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public Book? FindById(Guid id) { return _books.FirstOrDefault(book => book.Id == id); }


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		var query = _books.AsEnumerable();
		switch (filter)
		{
			case EntityFilter.Active:
				query = query.Where(b => !b.IsRemoved);
				break;
			case EntityFilter.Removed:
				query = query.Where(b => b.IsRemoved);
				break;
			case EntityFilter.All:
				// No filter – include everyone
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
		}

		return [.. query];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid authorId)
	{
		return [.. _books.Where(book => book.BookAuthors.Any(ba => ba.AuthorId == authorId))];
	}


	public bool ExistsByName(string name, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(name)) return false;

		return _books.Any(book => book.Id != excludeId && book.Title.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(string isbn, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(isbn)) return false;

		return _books.Any(book =>
			book.Id != excludeId &&
			book.InternationalStandardBookNumber.Equals(isbn, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Book> GetAvailableBooks()
	{
		return [.. _books.Where(b => b is { AvailableCopies: > 0, IsRemoved: false })];
	}


	public void Remove(Book book)
	{
		if (book.IsRemoved) return;
		book.IsRemoved = true;
		book.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector)
	{
		return
		[
			.. _books.Where(book =>
			{
				var value = selector(book);
				return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector)
	{
		return
		[
			.. _books.Where(book =>
			{
				var value = selector(book);
				return value >= from && value <= to;
			})
		];
	}


	public void Update(Book book, UpdateBookDto dto)
	{
		book.Title = dto.BookName ?? book.Title;
		book.InternationalStandardBookNumber = dto.ISBN ?? book.InternationalStandardBookNumber;
		Author
		translator
		book.PublishDate = dto.PublishDate ?? book.PublishDate;
		book.Genre = dto.Genre ?? book.Genre;
		book.Publisher = dto.Publisher ?? book.Publisher;

	}
}