using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

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
		return _session.IsAuthenticated && _session.Permissions.Contains(permission);
	}


	public bool HasAnyPermission(params Permission[] permissions) => permissions.Any(HasPermission);


	public ServiceResult<string> CheckBorrowEligibility(UserDto user)
	{
		if (!_session.IsAuthenticated) return ServiceResult<string>.Fail(Messages.AuthenticationRequired);

		if (!user.IsActive) return ServiceResult<string>.Fail(Messages.InactiveAccount);

		if (user.ShouldRemove) return ServiceResult<string>.Fail(Messages.UserAutoRemovedSuccessfully);

		if (user.MembershipExpiryDate < DateOnly.FromDateTime(DateTime.Today))
			return ServiceResult<string>.Fail(Messages.MembershipExpired);

		return _loanRepository.GetByUserId(user.Id, LoanFilter.Overdue).Count > 0
			? ServiceResult<string>.Fail(Messages.BorrowBlockedDueToOverdue)
			: ServiceResult<string>.Ok("Eligible to borrow.", "Borrowing allowed.");
	}


	public bool CanAccessContributorManagement()
	{
		return HasAnyPermission(Permission.AddContributor, Permission.EditContributor, Permission.RemoveContributor,
			Permission.SearchForMember, Permission.FullSearchContributor, Permission.FullSortContributor,
			Permission.ViewContributorDetails, Permission.ViewAllContributors);
	}


	public bool CanAccessBookManagement()
	{
		return HasAnyPermission(Permission.AddBook, Permission.EditBook, Permission.RemoveBook, Permission.SearchBook,
			Permission.FullSortBook, Permission.ViewBookDetails, Permission.ViewAllBooks);
	}


	public bool CanAccessFineManagement()
	{
		return HasAnyPermission(Permission.PayFine, Permission.WaiveFine, Permission.ViewFines,
			Permission.ViewFinesByUser, Permission.ViewUnpaidFines, Permission.ViewUnpaidFinesByUser,
			Permission.ViewFineHistory, Permission.FineHistoryByUser);
	}


	public bool CanAccessLoanManagement()
	{
		return HasAnyPermission(Permission.MyLoans, Permission.ViewLoansByUser, Permission.ViewLoansByBook,
			Permission.SearchLoans);
	}


	public bool CanAccessUserManagement()
	{
		return HasAnyPermission(Permission.AddUser, Permission.EditUser, Permission.RemoveUser, Permission.SearchUser,
			Permission.SortUser,
			Permission.ViewUserDetails, Permission.ViewAllUsers, Permission.ChangePassword,
			Permission.RenewLibrarianMembership, Permission.RenewMemberMembership);
	}


	public bool CanAccessAuditLogManagement() { return HasAnyPermission(Permission.ViewAuditLog); }


	public bool CanAccessStatistics() { return HasAnyPermission(Permission.ViewStatistics); }
}