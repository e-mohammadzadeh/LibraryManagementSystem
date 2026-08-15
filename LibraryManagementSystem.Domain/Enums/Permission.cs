namespace LibraryManagementSystem.Domain.Enums;

public enum Permission
{
	// Authors / Translators
	AddAuthor,
	EditAuthor,
	SearchAuthor,
	ViewAuthorDetails,
	ViewAuthorBooks,
	ViewAllAuthors,
	AddTranslator,
	EditTranslator,
	SearchTranslator,
	ViewTranslatorDetails,
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
	SearchLoans,
	


	// Users
	AddUser,
	EditUser,
	RemoveUser,
	SearchUser,
	ViewUserDetails,
	ViewAllUsers,
	ChangePassword,
	ChangeOwnPassword,


	// System
	ViewStatistics
}