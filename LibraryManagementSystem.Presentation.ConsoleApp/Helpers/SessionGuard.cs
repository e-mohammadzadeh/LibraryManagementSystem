using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class SessionGuard
{
	private static bool Fail(string message)
	{
		ConsoleHelper.ShowError(message);
		ConsoleHelper.Pause();
		return false;
	}


	public static bool RequireAuthentication(ICurrentUserSession session)
	{
		return session.IsAuthenticated || Fail(Messages.AuthenticationRequired);
	}


	public static bool RequirePermission(IAuthorizationService auth, Permission permission, string? message = null)
	{
		return auth.HasPermission(permission) || Fail(message ?? Messages.AccessDenied);
	}


	public static bool RequireAnyPermission(IAuthorizationService auth, string? message = null,
		params Permission[] permissions)
	{
		return auth.HasAnyPermission(permissions) || Fail(message ?? Messages.AccessDenied);
	}


	public static bool RequireAllPermissions(IAuthorizationService auth, string? message = null,
		params Permission[] permissions)
	{
		return permissions.All(auth.HasPermission) || Fail(message ?? Messages.AccessDenied);
	}
}