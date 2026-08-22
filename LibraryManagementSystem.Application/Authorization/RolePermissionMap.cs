using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	//private static readonly IReadOnlyList<Permission> AllPermissions = .ToHashSet().AsReadOnly();

	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = [.. Enum.GetValues<Permission>()],
		[LibraryUserRole.Librarian] =
		[
			Permission.AddAuthor,
			Permission.EditAuthor,
			Permission.SearchAuthor,
			Permission.SearchAuthorByName,
			Permission.SearchAuthorByNationalCode,
			Permission.SearchAuthorByEmail,
			Permission.SearchAuthorByPhoneNumber,
			Permission.ViewAuthorDetails,
			Permission.ViewAuthorBooks,
			Permission.ViewAllAuthors,

			Permission.AddTranslator,
			Permission.EditTranslator,
			Permission.SearchTranslator,
			Permission.SearchTranslatorByName,
			Permission.SearchTranslatorByNationalCode,
			Permission.SearchTranslatorByEmail,
			Permission.SearchTranslatorByPhoneNumber,
			Permission.ViewTranslatorDetails,
			Permission.ViewTranslatorBooks,
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
			Permission.MyActiveLoans,
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
			Permission.SearchUser,
			Permission.ViewUserDetails,
			Permission.ViewOwnDetails,
			Permission.ViewAllUsers,
			Permission.ChangePassword,
			Permission.RenewMemberMembership,

			Permission.ViewStatistics
		],

		[LibraryUserRole.Member] =
		[
			Permission.SearchAuthor,
			Permission.SearchAuthorByName,
			Permission.SearchAuthorByEmail,
			Permission.ViewAuthorBooks,
			Permission.ViewAllAuthors,

			Permission.SearchTranslator,
			Permission.SearchTranslatorByName,
			Permission.SearchTranslatorByEmail,
			Permission.ViewTranslatorBooks,
			Permission.ViewAllTranslators,

			Permission.SearchBook,
			Permission.ViewAllBooks,

			Permission.PayFine,
			Permission.ViewFines,
			Permission.ViewUnpaidFines,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.MyActiveLoans,
			Permission.MyOverdueLoans,
			Permission.MyFullLoanHistory,

			Permission.ViewOwnDetails,
			Permission.ChangeOwnPassword,
			Permission.AssignMemberRole
		]
	};


	public static IReadOnlySet<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : [];
}