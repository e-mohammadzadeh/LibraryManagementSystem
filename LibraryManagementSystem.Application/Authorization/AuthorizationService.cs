using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
	private readonly ICurrentUserSession _session;
	private readonly ILoanRepository _loanRepository;


	public AuthorizationService(ICurrentUserSession session, ILoanRepository loanRepository)
	{
		_session = session;
		_loanRepository = loanRepository;
	}


	public bool HasPermission(Permission permission)
	{
		if (!_session.IsAuthenticated || _session.CurrentUser is null) return false;

		return _session.Permissions.Contains(permission);
	}


	public bool HasAnyPermission(params Permission[] permissions) => permissions.Any(HasPermission);


	public ServiceResult<string> CheckBorrowEligibility(UserDto user)
	{
		if (!_session.IsAuthenticated) return ServiceResult<string>.Fail(Messages.AuthenticationRequired);

		if (!user.IsActive)
			return ServiceResult<string>.Fail(Messages.InactiveAccount);

		if (user.ShouldRemove)
			return ServiceResult<string>.Fail(Messages.UserAutoRemovedSuccessfully);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<string>.Fail(Messages.MembershipExpired);

		return _loanRepository.HasOverdueLoans(user.Id)
			? ServiceResult<string>.Fail(Messages.BorrowBlockedDueToOverdue)
			: ServiceResult<string>.Ok("Eligible to borrow.", "Borrowing allowed.");
	}


	public bool CanAccessAuthorManagement()
	{
		return HasAnyPermission(Permission.AddAuthor, Permission.EditAuthor, Permission.SearchAuthorForMember,
			Permission.FullSearchAuthor, Permission.SortAuthorForMember, Permission.FullSortAuthor,
			Permission.ViewAuthorDetails, Permission.ViewAllAuthors);
	}


	public bool CanAccessTranslatorManagement()
	{
		return HasAnyPermission(Permission.AddTranslator, Permission.EditTranslator,
			Permission.SearchTranslatorForMember, Permission.FullSearchTranslator, Permission.SortTranslatorForMember,
			Permission.FullSortTranslator, Permission.ViewTranslatorDetails, Permission.ViewAllTranslators);
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
			Permission.MyFullLoanHistory, Permission.SearchLoans);
	}


	public bool CanAccessUserManagement()
	{
		return HasAnyPermission(Permission.AddUser, Permission.EditUser, Permission.RemoveUser, Permission.SearchUser,
			Permission.ViewUserDetails, Permission.ViewOwnDetails, Permission.ViewOwnDetails, Permission.ViewAllUsers,
			Permission.ChangePassword, Permission.ChangeOwnPassword, Permission.RenewLibrarianMembership,
			Permission.RenewMemberMembership);
	}


	public bool canAccessAuditLogManagement()
	{
		return HasAnyPermission(Permission.ViewAuditLog);
	}


	public bool CanAccessStatistics() { return HasAnyPermission(Permission.ViewStatistics); }
}