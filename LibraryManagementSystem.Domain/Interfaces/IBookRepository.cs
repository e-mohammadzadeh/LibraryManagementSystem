using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookRepository
{
	void Add(Book book);
	Book? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<Book> GetAll(EntityFilter filter);
	IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter);
	IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter);
	bool Exists(Expression<Func<Book, bool>> predicate, Guid? excludeId = null);
	void Remove(Book book);
	void Update(Book book);
}