using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
	private readonly ICurrentUserSession _session;

	public AuthorizationService(ICurrentUserSession session) { _session = session; }


	public bool HasPermission(Permission permission)
	{
		if (!_session.IsAuthenticated || _session.CurrentUser is null) return false;

		return _session.Permissions.Contains(permission);
	}


	public bool HasAnyPermission(params Permission[] permissions) => permissions.Any(HasPermission);


	public bool CanBorrowBooks(UserDto? user)
	{
		if (!_session.IsAuthenticated || user is null) return false;

		return user is { IsActive: true, ShouldRemove: false } &&
		       user.MembershipExpiryDate >= DateOnly.FromDateTime(DateTime.Today);
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
		return HasAnyPermission(Permission.PayFine, Permission.WaiveFine, Permission.ViewFines,
			Permission.ViewFinesByUser, Permission.ViewUnpaidFines, Permission.ViewUnpaidFinesByUser,
			Permission.ViewFineHistory, Permission.FineHistoryByUser);
	}


	public bool CanAccessLoanManagement()
	{
		return HasAnyPermission(Permission.BorrowBook, Permission.ReturnBook, Permission.RenewLoan,
			Permission.ViewActiveLoans, Permission.ViewOverdueLoans, Permission.ViewLoanHistory,
			Permission.MyActiveLoans, Permission.ViewActiveLoansByUser, Permission.ViewActiveLoansByBook,
			Permission.MyOverdueLoans, Permission.ViewOverdueLoansByUser, Permission.ViewOverdueLoansByBook,
			Permission.LoanHistoryByUser, Permission.LoanHistoryByBook, Permission.FullLibraryHistory,
			Permission.SearchLoans);
	}


	public bool CanAccessUserManagement()
	{
		return HasAnyPermission(Permission.AddUser, Permission.EditUser, Permission.RemoveUser, Permission.SearchUser,
			Permission.ViewUserDetails, Permission.ViewAllUsers, Permission.ChangePassword,
			Permission.ChangeOwnPassword);
	}


	public bool CanAccessStatistics() { return HasAnyPermission(Permission.ViewStatistics); }
}