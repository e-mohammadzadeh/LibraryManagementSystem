using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
	private readonly ICurrentUserSession _session;

	public AuthorizationService(ICurrentUserSession session) { _session = session; }


	public bool HasPermission(Permission permission)
	{
		if (!_session.IsAuthenticated || _session.CurrentUser is null) return false;

		return _session.CurrentUser.Roles.SelectMany(RolePermissionMap.GetPermissions).Contains(permission);
	}


	public bool HasAnyPermission(params Permission[] permissions) => permissions.Any(HasPermission);


	public bool CanBorrowBooks()
	{
		return _session.IsAuthenticated && _session.CurrentUser!.IsActive && !_session.CurrentUser.ShouldRemove &&
		       _session.CurrentUser.MembershipExpiryDate >= DateOnly.FromDateTime(DateTime.Today);
	}


	public bool CanAccessAuthorManagement()
	{
		return HasAnyPermission(Permission.AddAuthor, Permission.EditAuthor, Permission.SearchAuthor,
			Permission.ViewAuthorDetails, Permission.ViewAllAuthors);
	}


	public bool CanAccessTranslatorManagement()
	{
		return HasAnyPermission(Permission.AddTranslator, Permission.EditTranslator, Permission.SearchTranslator,
			Permission.ViewTranslatorDetails, Permission.ViewAllTranslators);
	}


	public bool CanAccessBookManagement()
	{
		return HasAnyPermission(Permission.AddBook, Permission.EditBook, Permission.RemoveBook, Permission.SearchBook,
			Permission.ViewBookDetails, Permission.ViewAllBooks);
	}


	public bool CanAccessFineManagement()
	{
		return HasAnyPermission(Permission.ViewAllFines, Permission.ViewUnpaidFines, Permission.ViewUserFines,
			Permission.PayFine);
	}


	public bool CanAccessLoanManagement()
	{
		return HasAnyPermission(Permission.BorrowBook, Permission.ReturnBook, Permission.RenewLoan,
			Permission.ViewBorrowedBooks, Permission.ViewLoanHistory, Permission.ViewLoanHistory,
			Permission.ViewOverdueLoans, Permission.ViewUserLoans, Permission.SearchLoans);
	}


	public bool CanAccessUserManagement()
	{
		return HasAnyPermission(Permission.AddUser, Permission.EditUser, Permission.RemoveUser, Permission.SearchUser,
			Permission.ViewUserDetails, Permission.ViewAllUsers, Permission.ChangePassword);
	}


	public bool CanAccessStatistics() { return HasAnyPermission(Permission.ViewStatistics); }


	public bool IsAuthorized(params Permission[] permissions) {
		return permissions.Any(HasPermission);
	}
}