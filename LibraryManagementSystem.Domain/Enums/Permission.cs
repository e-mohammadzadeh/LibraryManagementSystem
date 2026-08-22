namespace LibraryManagementSystem.Domain.Enums;

public enum Permission
{
	// Authors / Translators
	AddAuthor,
	EditAuthor,
	SearchAuthor,
	SearchAuthorByName,
	SearchAuthorByNationalCode,
	SearchAuthorByEmail,
	SearchAuthorByPhoneNumber,
	ViewAuthorDetails,
	ViewAuthorBooks,
	ViewAllAuthors,
	AddTranslator,
	EditTranslator,
	SearchTranslator,
	SearchTranslatorByName,
	SearchTranslatorByNationalCode,
	SearchTranslatorByEmail,
	SearchTranslatorByPhoneNumber,
	ViewTranslatorDetails,
	ViewTranslatorBooks,
	ViewAllTranslators,
	RemoveAuthor,
	RemoveTranslator,

	
	// Books
	AddBook,
	EditBook,
	RemoveBook,
	SearchBook,
	ViewBookDetails,
	ViewAllBooks,


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
	ViewActiveLoans,
	ViewOverdueLoans,
	MyActiveLoans,
	ViewActiveLoansByUser,
	ViewActiveLoansByBook,
	MyOverdueLoans,
	ViewOverdueLoansByUser,
	ViewOverdueLoansByBook,
	ViewLoanHistory,
	LoanHistoryByUser,
	LoanHistoryByBook,
	FullLibraryHistory,
	MyFullLoanHistory,
	SearchLoans,
	


	// Users
	AddUser,
	AssignMemberRole,
	AssignLibrarianRole,
	AssignAdminRole,
	EditUser,
	ChangeUserRoles,
	RemoveUser,
	SearchUser,
	ViewUserDetails,
	ViewOwnDetails,
	ViewMemberDetails,
	ViewAllUsers,
	ChangePassword,
	ChangeOwnPassword,
	RenewLibrarianMembership,
	RenewMemberMembership,


	// System
	ViewStatistics
}