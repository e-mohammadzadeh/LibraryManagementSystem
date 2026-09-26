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
	IReadOnlyList<Book> GetByTranslatorId(Guid translatorId);
	bool ExistsByName(string name, Guid? excludeId);
	bool ExistsByISBN(string isbn, Guid? excludeId);
	IReadOnlyList<Book> GetAvailableBooks();
	void Remove(Book book);
	IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector);
	IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector);
	void Update(Book book, UpdateBookDto dto);
	void AssignAuthorsToBook(Book book, IEnumerable<Author> authors);
	void AssignTranslatorsToBook(Book book, IEnumerable<Translator> translators);
	void ReplaceAuthors(Book book, IEnumerable<Author> authors);
	void ReplaceTranslators(Book book, IEnumerable<Translator> translators);
	void BorrowCopy(Book book);
	void ReturnCopy(Book book);
}