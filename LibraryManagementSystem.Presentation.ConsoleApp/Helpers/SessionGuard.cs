using LibraryManagementSystem.Application.Authentication;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class SessionGuard
{
	public static bool RequireAuthentication(ICurrentUserSession session)
	{
		if (session.IsAuthenticated) return true;
		ConsoleHelper.ShowError("Access denied. You must be logged in.");
		return false;
	}


	public static bool RequireAdmin(ICurrentUserSession session)
	{
		if (session.IsAdmin) return true;
		ConsoleHelper.ShowError("Access denied. Admin role required.");
		return false;
	}


	public static bool RequireLibrarian(ICurrentUserSession session)
	{
		if (session.IsLibrarian) return true;
		ConsoleHelper.ShowError("Access denied. Librarian role required.");
		return false;
	}


	public static bool RequireMember(ICurrentUserSession session)
	{
		if (session.IsMember) return true;
		ConsoleHelper.ShowError("Access denied. Member role required.");
		return false;
	}


	public static bool RequireCanBorrow(ICurrentUserSession session)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError("Access denied. You must be logged in.");
			return false;
		}

		if (!session.CanBorrowBooks)
		{
			ConsoleHelper.ShowError("You cannot borrow books. Check your membership status or outstanding fines.");
			return false;
		}

		return true;
	}
}