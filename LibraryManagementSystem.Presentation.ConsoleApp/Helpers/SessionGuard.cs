using LibraryManagementSystem.Application.Authentication;
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
		return RequireRole(session.IsAuthenticated, "You must be logged in.");
	}


	public static bool RequireAdmin(ICurrentUserSession session)
	{
		return RequireRole(session.IsAdmin, "Admin role required.");
	}


	public static bool RequireLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.IsLibrarian, "Librarian role required.");
	}


	public static bool RequireMember(ICurrentUserSession session)
	{
		return RequireRole(session.IsMember, "Member role required.");
	}


	public static bool RequireAdminOrLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.IsAdmin || session.IsLibrarian, "Admin or Librarian role required.");
	}


	public static bool RequireCanBorrow(ICurrentUserSession session)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError("Access denied. You must be logged in.");
			ConsoleHelper.Pause();
			return false;
		}

		if (!session.HasBasicBorrowPermission)
		{
			ConsoleHelper.ShowError("You cannot borrow books. Check your membership status or outstanding fines.");
			ConsoleHelper.Pause();
			return false;
		}

		return true;
	}


	public static bool RequireAdminOrLibrarian(ICurrentUserSession session)
	{
		if (session.HasAnyRole(LibraryUserRole.Admin, LibraryUserRole.Librarian))
			return true;

		ConsoleHelper.ShowError("Access denied. Admin or Librarian role required.");
		ConsoleHelper.Pause();
		return false;
	}
}