using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public interface IAuthorizationService
{
	bool HasPermission(Permission permission);
	bool HasAnyPermission(params Permission[] permissions);
	bool CanBorrowBooks();
	bool CanAccessAuthorManagement();
	bool CanAccessUserManagement();
	bool CanAccessBookManagement();
	bool CanAccessTranslatorManagement();
	bool CanAccessLoanManagement();
	bool CanAccessFineManagement();
	bool CanAccessStatistics();
}