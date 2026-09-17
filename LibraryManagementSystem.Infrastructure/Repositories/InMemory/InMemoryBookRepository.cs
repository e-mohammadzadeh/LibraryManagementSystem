using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);
		_books.Add(book);
	}


	public Book? FindById(int id) { return _books.FirstOrDefault(b => b.Id == id); }


	public IReadOnlyList<Book> GetAll() { return _books.AsReadOnly(); }
	public IReadOnlyList<Book> GetByAuthorId(Guid authorId)
	{
		return [.. _books.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))];
	}


	public bool ExistsByName(string name, int excludeBookId = -1)
	{
		if (string.IsNullOrWhiteSpace(name)) return false;

		return _books.Any(b =>
			b.Id != excludeBookId && b.Title.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(string isbn, int excludeBookId = -1)
	{
		if (string.IsNullOrWhiteSpace(isbn)) return false;

		return _books.Any(b =>
			b.Id != excludeBookId &&
			b.InternationalStandardBookNumber.Equals(isbn, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Book> GetAvailableBooks() { return [.. _books.Where(b => b.AvailableCopies > 0)]; }


	public void Remove(Book book) { _books.Remove(book); }


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


	public void Update(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);
		var existingBookIndex = _books.FindIndex(b => b.Id == book.Id);
		if (existingBookIndex == -1) throw new KeyNotFoundException($"Book with ID {book.Id} was not found.");
		_books[existingBookIndex] = book;
	}
}