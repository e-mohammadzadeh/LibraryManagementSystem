using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookAuthorRepository
{
	void Add(BookAuthor bookAuthor);
	void Remove(BookAuthor bookAuthor);
	void Update(BookAuthor bookAuthor);

	BookAuthor? GetByBookId(Guid bookId, EntityFilter filter);
	BookAuthor? GetByAuthorId(Guid authorId, EntityFilter filter);
	IReadOnlyList<BookAuthor> GetAll(EntityFilter filter);

	bool Exists<T>(T value, Func<BookAuthor, T> selector, Guid? excludeId = null);
}