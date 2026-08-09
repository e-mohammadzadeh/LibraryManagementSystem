using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	private static readonly HashSet<Permission> AllPermissions =
		Enum.GetValues<Permission>().ToHashSet();

	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = AllPermissions,
		[LibraryUserRole.Librarian] =
		[
			Permission.AddAuthor,
			Permission.EditAuthor,
			Permission.SearchAuthor,
			Permission.ViewAuthorDetails,
			Permission.ViewAllAuthors,

			Permission.AddTranslator,
			Permission.EditTranslator,
			Permission.SearchTranslator,
			Permission.ViewTranslatorDetails,
			Permission.ViewAllTranslators,

			Permission.AddBook,
			Permission.EditBook,
			Permission.RemoveBook,
			Permission.SearchBook,
			Permission.ViewBookDetails,
			Permission.ViewAllBooks,

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
			Permission.ViewActiveLoansByUser,
			Permission.ViewActiveLoansByBook,
			Permission.ViewOverdueLoansByUser,
			Permission.ViewOverdueLoansByBook,
			Permission.LoanHistoryByUser,
			Permission.LoanHistoryByBook,
			Permission.FullLibraryHistory,
			Permission.SearchLoans,

			Permission.AddUser,
			Permission.EditUser,
			Permission.RemoveUser,
			Permission.SearchUser,
			Permission.ViewUserDetails,
			Permission.ViewAllUsers,
			Permission.ChangePassword,

			Permission.ViewStatistics
		],

		[LibraryUserRole.Member] =
		[
			Permission.SearchAuthor,
			Permission.ViewAuthorDetails,
			Permission.ViewAllAuthors,

			Permission.SearchTranslator,
			Permission.ViewTranslatorDetails,
			Permission.ViewAllTranslators,

			Permission.SearchBook,
			Permission.ViewBookDetails,
			Permission.ViewAllBooks,

			Permission.PayFine,
			Permission.ViewFines,
			Permission.ViewUnpaidFines,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewActiveLoans,
			Permission.ViewOverdueLoans,
			Permission.MyActiveLoans,
			Permission.MyOverdueLoans,

			Permission.ChangeOwnPassword
		]
	};


	public static IReadOnlySet<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : [];
}