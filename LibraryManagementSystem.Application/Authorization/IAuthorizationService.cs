using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public interface IAuthorizationService
{
	bool HasPermission(Permission permission);
	bool HasAnyPermission(params Permission[] permissions);
	ServiceResult<string> CheckBorrowEligibility(UserDto user);
	bool CanAccessAuthorManagement();
	bool CanAccessTranslatorManagement();
	bool CanAccessBookManagement();
	bool CanAccessFineManagement();
	bool CanAccessLoanManagement();
	bool CanAccessUserManagement();
	bool CanAccessStatistics();
}