using LibraryManagementSystem.Application.Authentication;
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
		return RequireRole(session.IsAuthenticated, ValidationMessages.AuthenticationRequired);
	}


	public static bool RequireAdmin(ICurrentUserSession session)
	{
		return RequireRole(session.IsAdmin, ValidationMessages.AdminRoleRequired);
	}


	public static bool RequireLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.IsLibrarian, ValidationMessages.LibrarianRoleRequired);
	}


	public static bool RequireMember(ICurrentUserSession session)
	{
		return RequireRole(session.IsMember, ValidationMessages.MemberRoleRequired);
	}



	public static bool RequireAdminOrLibrarian(ICurrentUserSession session)
	{
		return RequireRole(session.HasAnyRole(LibraryUserRole.Admin, LibraryUserRole.Librarian),
			ValidationMessages.AdminOrLibrarianRoleRequired);
	}



	public static bool RequireCanBorrow(ICurrentUserSession session)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError(ValidationMessages.AuthenticationRequired);
			ConsoleHelper.Pause();
			return false;
		}

		if (!session.CanBorrowBooks)
		{
			ConsoleHelper.ShowError(ValidationMessages.CannotBorrowBooks);
			ConsoleHelper.Pause();
			return false;
		}

		return true;
	}


	public static bool RequireAuthorManagement(ICurrentUserSession session)
	{
		return RequireRole(session.CanAccessAuthorManagement,
			"Access denied. You need Admin or Librarian privileges to manage authors.");
	}


	public static bool RequireTranslatorManagement(ICurrentUserSession session)
	{
		return RequireRole(session.CanAccessTranslatorManagement,
			"Access denied. You need Admin or Librarian privileges to manage translators.");
	}


	public static bool RequireUserManagement(ICurrentUserSession session)
	{
		return RequireRole(session.CanAccessUserManagement, ValidationMessages.AuthenticationRequired);
	}


	public static bool RequireBookManagement(ICurrentUserSession session)
	{
		return RequireRole(session.CanAccessBookManagement,
			"Access denied. You need Admin or Librarian privileges to manage books.");
	}
}