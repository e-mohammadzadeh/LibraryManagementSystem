using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookAuthorRepository
{
	IReadOnlyList<BookAuthor>? GetByBookId(Guid id, EntityFilter filter);
	IReadOnlyList<BookAuthor>? GetByAuthorId(Guid id, EntityFilter filter);

}