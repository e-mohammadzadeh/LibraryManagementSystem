using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Infrastructure.DTOs.Books;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookRepository
{
	void Add(Book book);
	Book? FindById(Guid id);
	IReadOnlyList<Book> GetAll(EntityFilter filter);
	IReadOnlyList<Book> GetByAuthorId(Guid authorId);
	bool ExistsByName(string name, Guid? excludeId);
	bool ExistsByISBN(string isbn, Guid? excludeId);
	IReadOnlyList<Book> GetAvailableBooks();
	void Remove(Book book);
	//	TODO Split search methods into some methods to support search books by author - search books by translator - search books by publisher - search books by genre - search books by multiple authors
	IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector);
	IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector);
	void Update(Book book, UpdateBookDto dto);
}