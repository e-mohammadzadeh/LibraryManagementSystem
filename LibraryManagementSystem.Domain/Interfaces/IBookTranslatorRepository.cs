using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IBookTranslatorRepository
{
	IReadOnlyList<BookTranslator>? GetByBookId(Guid id, EntityFilter filter);
	IReadOnlyList<BookTranslator>? GetByTranslatorId(Guid id, EntityFilter filter);
}