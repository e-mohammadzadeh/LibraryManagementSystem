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


	ViewAuthorDetails,
	ViewAuthorBooks,
	ViewAllAuthors,
	ViewRemovedAuthors,
	ViewAuthorFullDetails,
	

	// Translators
	ViewTranslatorDetails,
	ViewTranslatorBooks,
	ViewAllTranslators,
	ViewRemovedTranslators,
	ViewTranslatorFullDetails,

	
	// Books
	AddBook,
	EditBook,
	RemoveBook,
	SearchBook,
	SortBookForMember,
	FullSortBook,
	ViewBookDetails,
	ViewBookAuthors,
	ViewAllBooks,
	ViewRemovedBooks,



	// Users
	AddUser,
	AssignMemberRole,
	AssignLibrarianRole,
	AssignAdminRole,
	EditUser,
	EditOwnProfile,
	EditUserRoles,
	RemoveUser,
	SearchUser,
	SortUser,
	ViewUserDetails,
	ViewOwnDetails,
	ViewAllUsers,
	ViewRemovedUsers,
	ChangePassword,
	ChangeOwnPassword,
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
	BorrowBook,
	ReturnBook,
	RenewLoan,
	ViewAllLoans,
	ViewActiveLoans,
	ViewInactiveLoans,
	ViewOverdueLoans,
	MyActiveLoans,
	MyInactiveLoans,
	ViewActiveLoansByUser,
	ViewInactiveLoansByUser,
	ViewActiveLoansByBook,
	ViewInactiveLoansByBook,
	MyOverdueLoans,
	ViewOverdueLoansByUser,
	ViewOverdueLoansByBook,
	ViewLoanHistory,
	LoanHistoryByUser,
	LoanHistoryByBook,
	FullLibraryHistory,
	MyFullLoanHistory,
	SearchLoans,


	// System
	ViewStatistics,
	ViewAuditLog
}