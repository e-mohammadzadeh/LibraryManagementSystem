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
	ViewBorrowedBooks,
	ViewLoanHistory,
	ViewOverdueLoans,
	ViewUserLoans,
	SearchLoans,
	ViewOwnBorrowedBooks,
	ViewOwnLoanHistory,
	ViewOwnOverdueLoans,
	ViewOwnLoans,


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