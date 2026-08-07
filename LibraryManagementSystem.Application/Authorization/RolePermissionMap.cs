using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = Enum.GetValues<Permission>().ToHashSet(),
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
			Permission.ViewAllFines,
			Permission.ViewUnpaidFines,
			Permission.ViewUserFines,
			Permission.PayFine,
			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewBorrowedBooks,
			Permission.ViewLoanHistory,
			Permission.ViewOverdueLoans,
			Permission.ViewUserLoans,
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
			Permission.ViewOwnUnpaidFines,
			Permission.ViewOwnFines,
			Permission.PayOwnFine,
			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewOwnBorrowedBooks,
			Permission.ViewOwnLoanHistory,
			Permission.ViewOwnOverdueLoans,
			Permission.ViewOwnLoans,
			Permission.ChangeOwnPassword
		]
	};


	public static IReadOnlySet<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : new HashSet<Permission>();
}