using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

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
}