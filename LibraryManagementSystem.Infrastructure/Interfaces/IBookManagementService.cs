using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IBookManagementService
{
	void AssignAuthorsToBook(Guid bookId, List<Guid> authors);
	void AssignTranslatorsToBook(Guid bookId, List<Guid>? translators, Language? language);
	void BorrowCopy(Guid bookId);
	void ReturnCopy(Guid bookId);
}