using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class SessionGuard
{
	public static bool RequireAuthentication(ICurrentUserSession session)
	{
		if (session.IsAuthenticated) return true;
		ConsoleHelper.ShowError("Access denied. You must be logged in.");
		Pause();
		return false;
	}


	public static bool RequireAdmin(ICurrentUserSession session)
	{
		if (session.IsAdmin) return true;
		ConsoleHelper.ShowError("Access denied. Admin role required.");
		Pause();
		return false;
	}


	public static bool RequireLibrarian(ICurrentUserSession session)
	{
		if (session.IsLibrarian) return true;
		ConsoleHelper.ShowError("Access denied. Librarian role required.");
		Pause();
		return false;
	}


	public static bool RequireMember(ICurrentUserSession session)
	{
		if (session.IsMember) return true;
		ConsoleHelper.ShowError("Access denied. Member role required.");
		Pause();
		return false;
	}

	public static bool RequireAdminOrLibrarian(ICurrentUserSession session) 
	{
		if (session.IsAdmin || session.IsLibrarian)
			return true;

		ConsoleHelper.ShowError("Access denied. Admin or Librarian role required.");
		Pause();
		return false;
	}


	public static bool RequireCanBorrow(ICurrentUserSession session)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError("Access denied. You must be logged in.");
			Pause();
			return false;
		}

		if (!session.CanBorrowBooks)
		{
			ConsoleHelper.ShowError("You cannot borrow books. Check your membership status or outstanding fines.");
			Pause();
			return false;
		}

		return true;
	}
}