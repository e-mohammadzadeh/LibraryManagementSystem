using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

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
	bool CanAccessAuditLogManagement();
	bool CanAccessStatistics();
}