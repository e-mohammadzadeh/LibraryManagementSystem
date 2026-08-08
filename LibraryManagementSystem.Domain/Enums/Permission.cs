namespace LibraryManagementSystem.Domain.Enums;

public enum Permission
{
	// Authors / Translators
	AddAuthor,
	EditAuthor,
	SearchAuthor,
	ViewAuthorDetails,
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
	ViewAllFines,
	ViewUnpaidFines,
	ViewUserFines,
	PayFine,
	WaiveFine,
	ViewOwnUnpaidFines,
	ViewOwnFines,
	PayOwnFine,


	// Loans
	BorrowBook,
	ReturnBook,
	RenewLoan,
	ViewActiveLoans,
	ViewOverdueLoans,
	ViewLoanHistory,
	MyActiveLoans,
	ViewActiveLoansByUser,
	ViewActiveLoansByBook,
	MyOverdueLoans,
	ViewOverdueLoansByUser,
	ViewOverdueLoansByBook,
	HistoryByUser,
	HistoryByBook,
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