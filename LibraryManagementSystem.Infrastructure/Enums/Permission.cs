namespace LibraryManagementSystem.Infrastructure.Enums;

public enum Permission
{
	// Contributors
	AddContributor,
	EditContributor,
	RemoveContributor,
	SearchForMember,
	FullSearchContributor,
	FullSortContributor,
	ViewContributorDetails,
	ViewAllContributors,
	ViewRemovedContributors,
	ViewContributorBooks,


	
	// Books
	AddBook,
	EditBook,
	RemoveBook,
	SearchBook,
	FullSortBook,
	ViewBookDetails,
	ViewAllBooks,
	ViewRemovedBooks,



	// Users
	AddUser,
	EditUser,
	EditOwnProfile,
	EditUserRoles,
	RemoveUser,
	SearchUser,
	SortUser,
	AssignMemberRole,
	AssignLibrarianRole,
	AssignAdminRole,
	ViewUserDetails,
	ViewAllUsers,
	ViewRemovedUsers,
	ChangePassword,
	RenewLibrarianMembership,
	RenewMemberMembership,
	RenewInactiveMembership,



	// Fines
	PayFine,
	WaiveFine,
	ViewFines,
	ViewFinesByUser,
	ViewUnpaidFines,
	ViewUnpaidFinesByUser,
	ViewFineHistory,
	FineHistoryByUser,


	// Loans
	MyLoans,
	ViewAllLoans,
	ViewLoansByUser,
	ViewLoansByBook,
	SearchLoans,


	// System
	ViewStatistics,
	ViewAuditLog
}