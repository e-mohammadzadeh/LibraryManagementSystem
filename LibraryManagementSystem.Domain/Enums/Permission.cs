namespace LibraryManagementSystem.Domain.Enums;

public enum Permission
{
	// Books
	ViewBooks,
	AddBook,
	EditBook,
	RemoveBook,

	// Authors / Translators
	ManageAuthors,
	ManageTranslators,
	RemoveAuthor,
	RemoveTranslator,

	// Loans
	BorrowBook,
	ReturnBook,
	RenewLoan,
	ViewOwnLoans,
	ViewAllLoans,

	// Fines
	ViewOwnFines,
	ViewAllFines,
	PayFine,
	WaiveFine,

	// Users
	ManageUsers,
	RemoveUser,
	ViewAllUsers,

	// System
	ViewStatistics
}