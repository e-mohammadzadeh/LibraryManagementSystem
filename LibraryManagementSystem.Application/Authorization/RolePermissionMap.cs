using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = [.. Enum.GetValues<Permission>()],
		[LibraryUserRole.Librarian] =
		[
			Permission.AddAuthor,
			Permission.EditAuthor,
			Permission.FullSearchAuthor,
			Permission.FullSortAuthor,
			Permission.ViewAuthorDetails,
			Permission.ViewAuthorBooks,
			Permission.ViewAllAuthors,
			Permission.ViewAuthorFullDetails,


			Permission.AddTranslator,
			Permission.EditTranslator,
			Permission.FullSearchTranslator,
			Permission.FullSortTranslator,
			Permission.ViewTranslatorDetails,
			Permission.ViewTranslatorBooks,
			Permission.ViewAllTranslators,
			Permission.ViewTranslatorFullDetails,


			Permission.AddBook,
			Permission.EditBook,
			Permission.RemoveBook,
			Permission.SearchBook,
			Permission.FullSortBook,
			Permission.ViewBookDetails,
			Permission.ViewAllBooks,


			Permission.AddUser,
			Permission.EditUser,
			Permission.SearchUser,
			Permission.SortUser,
			Permission.ViewUserDetails,
			Permission.ViewOwnDetails,
			Permission.ViewAllUsers,
			Permission.ChangeOwnPassword,
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
			Permission.ViewOverdueLoans,
			Permission.ViewLoanHistory,
			Permission.MyActiveLoans,
			Permission.ViewActiveLoansByUser,
			Permission.ViewActiveLoansByBook,
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
			Permission.SearchAuthorForMember,
			Permission.SortAuthorForMember,
			Permission.ViewAuthorBooks,
			Permission.ViewAllAuthors,

			Permission.SearchTranslatorForMember,
			Permission.SortTranslatorForMember,
			Permission.ViewTranslatorBooks,
			Permission.ViewAllTranslators,

			Permission.SearchBook,
			Permission.SortBookForMember,
			Permission.ViewAllBooks,

			Permission.PayFine,
			Permission.ViewFines,
			Permission.ViewUnpaidFines,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.MyActiveLoans,
			Permission.MyOverdueLoans,
			Permission.ViewFineHistory,
			Permission.MyFullLoanHistory,

			Permission.ViewOwnDetails,
			Permission.ChangeOwnPassword,
			Permission.AssignMemberRole
		]
	};


	public static IReadOnlySet<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : [];
}