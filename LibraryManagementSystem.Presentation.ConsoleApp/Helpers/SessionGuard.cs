using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class SessionGuard
{
	private static bool RequireRole(bool hasPermission, string message)
	{
		if (hasPermission) return true;

		ConsoleHelper.ShowError(message);
		ConsoleHelper.Pause();
		return false;
	}


	public static bool RequireAuthentication(ICurrentUserSession session)
	{
		return RequireRole(session.IsAuthenticated, Messages.AuthenticationRequired);
	}


	public static bool RequireAdmin(ICurrentUserSession session)
	{
		return RequireRole(session.IsAdmin, Messages.AdminRoleRequired);
	}


	public static bool RequireLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.IsLibrarian, Messages.LibrarianRoleRequired);
	}


	public static bool RequireMember(ICurrentUserSession session)
	{
		return RequireRole(session.IsMember, Messages.MemberRoleRequired);
	}



	public static bool RequireAdminOrLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.HasAnyRole(LibraryUserRole.Admin, LibraryUserRole.Librarian),
			Messages.AdminOrLibrarianRoleRequired);
	}


	public static bool RequirePermission(IAuthorizationService auth, Permission permission, string? message = null)
	{
		if (auth.HasPermission(permission)) return true;

		ConsoleHelper.ShowError(message ?? Messages.AccessDenied);
		ConsoleHelper.Pause();
		return false;
	}


	public static bool RequireAnyPermission(IAuthorizationService auth, string message, params Permission[] permission)
	{
		if (auth.HasAnyPermission(permission)) return true;

		ConsoleHelper.ShowError(message ?? "Access denied.");
		ConsoleHelper.Pause();
		return false;
	}
}