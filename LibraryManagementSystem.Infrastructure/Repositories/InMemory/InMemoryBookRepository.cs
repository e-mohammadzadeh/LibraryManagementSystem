using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = new();

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


	public void Remove(Book book)
	{
		_books.Remove(book);
	}


	public IReadOnlyList<Book> Search<T>(T searchItem, Func<Book, T> selector, Func<T, T, bool> comparer)
		where T : class
	{
		if (searchItem is null)
			return new List<Book>();

		return _books.Where(book =>
		{
			var value = selector(book);
			return value is not null && comparer(searchItem, value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<Book> Search<T>(T? searchItem, Func<Book, T?> selector, Func<T?, T?, bool> comparer)
		where T : struct
	{
		if (!searchItem.HasValue)
			return new List<Book>();

		return _books.Where(book =>
		{
			var value = selector(book);
			return value.HasValue && comparer(searchItem.Value, value.Value);
		}).ToList().AsReadOnly();
	}
}