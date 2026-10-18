using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookRepository
{
	void Add(Book book);
	Book? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<Book> GetAll(EntityFilter filter);
	IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter);
	IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter);
	IReadOnlyList<Book> GetAvailableBooks(EntityFilter filter);
	bool Exists<T>(T value, Func<Book, T> selector, Guid? excludeId = null);
	void Remove(Book book);
	void Update(Book book);
}