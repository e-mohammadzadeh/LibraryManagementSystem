using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.DTOs.Books;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookRepository
{
	void Add(Book book);
	Book? FindById(Guid id, EntityFilter filter);
	IReadOnlyList<Book> GetAll(EntityFilter filter);
	IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter);
	IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter);
	IReadOnlyList<Book> GetAvailableBooks(EntityFilter filter);
	bool ExistsByName(string name, Guid? excludeId);
	bool ExistsByISBN(ISBN isbn, Guid? excludeId);
	void Remove(Book book);
	IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector);
	IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector);
	void Update(Book book, Guid? updatedBy);
	void AssignAuthorsToBook(Book book, IEnumerable<Author> authors);
	void AssignTranslatorsToBook(Book book, IEnumerable<Translator> translators);
	void ReplaceAuthors(Book book, IEnumerable<Author> authors);
	void ReplaceTranslators(Book book, IEnumerable<Translator> translators);
	void BorrowCopy(Book book);
	void ReturnCopy(Book book);
}