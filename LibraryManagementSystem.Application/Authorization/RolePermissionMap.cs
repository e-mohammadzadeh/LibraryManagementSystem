using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = [.. Enum.GetValues<Permission>()],
		[LibraryUserRole.Librarian] =
		[
			Permission.AddContributor,
			Permission.EditContributor,
			Permission.FullSearchContributor,
			Permission.FullSortContributor,
			Permission.ViewContributorDetails,
			Permission.ViewAllContributors,
			Permission.ViewContributorBooks,
			

			Permission.AddBook,
			Permission.EditBook,
			Permission.RemoveBook,
			Permission.SearchBook,
			Permission.FullSortBook,
			Permission.ViewBookDetails,
			Permission.ViewAllBooks,


			Permission.AddUser,
			Permission.EditUser,
			Permission.EditOwnProfile,
			Permission.SearchUser,
			Permission.SortUser,
			Permission.ViewUserDetails,
			Permission.ViewAllUsers,
			Permission.RenewMemberMembership,


			Permission.PayFine,
			Permission.ViewFines,
			Permission.ViewFinesByUser,
			Permission.ViewUnpaidFines,
			Permission.ViewUnpaidFinesByUser,
			Permission.ViewFineHistory,
			Permission.FineHistoryByUser,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewActiveLoans,
			Permission.ViewInactiveLoans,
			Permission.ViewOverdueLoans,
			Permission.ViewLoanHistory,
			Permission.MyActiveLoans,
			Permission.MyInactiveLoans,
			Permission.ViewActiveLoansByUser,
			Permission.ViewInactiveLoansByUser,
			Permission.ViewActiveLoansByBook,
			Permission.ViewInactiveLoansByBook,
			Permission.ViewOverdueLoansByUser,
			Permission.ViewOverdueLoansByBook,
			Permission.LoanHistoryByUser,
			Permission.LoanHistoryByBook,
			Permission.FullLibraryHistory,
			Permission.SearchLoans,

			Permission.ViewStatistics
		],

		[LibraryUserRole.Member] =
		[
			Permission.SearchForMember,
			Permission.ViewContributorBooks,
			Permission.ViewAllContributors,

			Permission.SearchBook,
			Permission.ViewAllBooks,

			Permission.PayFine,
			Permission.ViewFines,
			Permission.ViewUnpaidFines,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.MyActiveLoans,
			Permission.MyInactiveLoans,
			Permission.MyOverdueLoans,
			Permission.ViewFineHistory,
			Permission.MyFullLoanHistory,

			Permission.EditOwnProfile,
			Permission.AssignMemberRole
		]
	};


	public static IReadOnlySet<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : [];
}