using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public static class RolePermissionMap
{
	private static readonly Dictionary<LibraryUserRole, HashSet<Permission>> Map = new()
	{
		[LibraryUserRole.Admin] = Enum.GetValues<Permission>().ToHashSet(),
		[LibraryUserRole.Librarian] =
		[
			Permission.ViewBooks,
			Permission.AddBook,
			Permission.EditBook,

			Permission.ManageAuthors,
			Permission.ManageTranslators,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewOwnLoans,
			Permission.ViewAllLoans,

			Permission.ViewOwnFines,
			Permission.ViewAllFines,
			Permission.PayFine,

			Permission.ManageUsers,
			Permission.ViewAllUsers,

			Permission.ViewStatistics
		],

		[LibraryUserRole.Member] =
		[
			Permission.ViewBooks,

			Permission.BorrowBook,
			Permission.ReturnBook,
			Permission.RenewLoan,
			Permission.ViewOwnLoans,

			Permission.ViewOwnFines,
			Permission.PayFine
		]
	};


	public static IReadOnlyList<Permission> GetPermissions(LibraryUserRole role) =>
		Map.TryGetValue(role, out var set) ? set : new HashSet<Permission>();
}