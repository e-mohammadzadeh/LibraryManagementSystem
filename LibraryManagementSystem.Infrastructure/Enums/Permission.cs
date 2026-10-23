namespace LibraryManagementSystem.Infrastructure.Enums;

public enum Permission
{
	// Authors
	AddAuthor,
	EditAuthor,
	SearchAuthorForMember,
	FullSearchAuthor,
	SortAuthorForMember,
	FullSortAuthor,
	ViewAuthorDetails,
	ViewAuthorBooks,
	ViewAllAuthors,
	ViewRemovedAuthors,
	ViewAuthorFullDetails,
	RemoveAuthor,
	

	// Translators
	AddTranslator,
	EditTranslator,
	SearchTranslatorForMember,
	FullSearchTranslator,
	SortTranslatorForMember,
	FullSortTranslator,
	ViewTranslatorDetails,
	ViewTranslatorBooks,
	ViewAllTranslators,
	ViewRemovedTranslators,
	ViewTranslatorFullDetails,
	RemoveTranslator,

	
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
	ChangeUserRoles,
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