using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];

	public void Add(Book book)
	{
		_books.Add(book);
	}


	public Book? FindById(int id)
	{
		return _books.FirstOrDefault(b => b.BookId == id);
	}


	public IReadOnlyList<Book> GetAll()
	{
		return _books.AsReadOnly();
	}


	public bool ExistsByName(string name, int excludeBookId = -1)
	{
		if (string.IsNullOrWhiteSpace(name))
			return false;

		return _books.Any(b =>
			b.BookId != excludeBookId && b.BookName.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(string isbn, int excludeBookId = -1)
	{
		if (string.IsNullOrWhiteSpace(isbn))
			return false;

		return _books.Any(b =>
			b.BookId != excludeBookId &&
			b.InternationalStandardBookNumber.Equals(isbn, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Book> GetAvailableBooks()
	{
		return _books.Where(b => b.AvailableCopies > 0).ToList().AsReadOnly();
	}


	public void Remove(Book book)
	{
		_books.Remove(book);
	}


	public IReadOnlyList<Book> Search<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		if (searchTerm is null)
			return [];

		return _books.Where(book =>
		{
			var value = selector(book);
			return value is not null && comparer(searchTerm, value);
		}).ToList().AsReadOnly();
	}



	public IReadOnlyList<Book> Search<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchTerm.HasValue)
			return [];

		return _books.Where(book =>
		{
			var value = selector(book);
			return value.HasValue && comparer(searchTerm.Value, value.Value);
		}).ToList().AsReadOnly();
	}
}