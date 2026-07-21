using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookRepository
{
	void Add(Book book);
	Book? FindById(int id);
	IReadOnlyList<Book> GetAll();

	bool ExistsByName(string name, int excludeBookId = -1);
	bool ExistsByISBN(string isbn, int excludeBookId = -1);

	IReadOnlyList<Book> GetAvailableBooks();
	void Remove(Book book);
	//	TODO Split search methods into some methods to support search books by author - search books by translator - search books by publisher - search books by genre - search books by multiple authors
	IReadOnlyList<Book> Search<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer) where T : class;
	IReadOnlyList<Book> Search<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer) where T : struct;
	int Count();
}