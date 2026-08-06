using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public interface IAuthorizationService
{
	bool HasPermission(Permission permission);
	bool HasAnyPermission(params Permission[] permissions);
}