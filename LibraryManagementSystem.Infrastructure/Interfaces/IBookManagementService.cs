using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IBookManagementService
{
	void BorrowCopy(Guid bookId);
	void ReturnCopy(Guid bookId);
}