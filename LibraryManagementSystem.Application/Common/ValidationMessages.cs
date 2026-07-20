namespace LibraryManagementSystem.Application.Common;

public static class ValidationMessages
{
	// Input Messages
	public const string GetRole = "Select a role for this user";


	// Invalid Messages
	public const string InvalidNationalCode = "Invalid national code. Please enter exactly 10 digits without dashes or spaces.";
	public const string InvalidEmail = "Invalid email address. Please enter a valid email (e.g., name@domain.com).";
	public const string InvalidPhoneNumber = "Invalid phone number. Please enter exactly 11 digits.";
	public const string InvalidBirthDate = "Invalid birth date. Please enter a past date that is no more than 120 years ago.";
	public const string InvalidDate = "Invalid date.  Please enter a date on or before today.";
	public const string InvalidBirthDateFormat = "Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).";
	public const string InvalidBookName = "Invalid book name. Please enter a name between 3 and 100 characters.";
	public const string InvalidGenre = "Invalid genre. Please select a valid genre.";
	public const string InvalidISBN = "Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.";
	public const string InvalidNumber = "Invalid number. Please try again.";
	public const string InvalidInput = "Invalid input. Please try again.";
	public const string InvalidYesNo = "Invalid input. Please enter 'y' or 'n'.";
	public const string InvalidMenuChoice = "Invalid selection, Try again.\n";
	public const string InvalidCharacters = "The name can only contain letters, numbers, spaces, and basic punctuation.";
	public const string InvalidRoleSelection = "Please enter at least one role number.";



	// Adding Messages
	public const string AuthorAddedSuccessfully = "Author added successfully.";
	public const string TranslatorAddedSuccessfully = "Translator added successfully.";
	public const string BookAddedSuccessfully = "Book added successfully.";
	public const string AuthorAddFailed = "Failed to add author.";
	public const string BookAddFailed = "Failed to add book.";
	public const string UserAddedSuccessfully = "User added successfully.";
	public const string UserAddFailed = "Failed to add user.";


	// Updating Messages
	public const string AuthorUpdatedSuccessfully = "Author updated successfully.";
	public const string TranslatorUpdatedSuccessfully = "Translator updated successfully.";
	public const string BookUpdatedSuccessfully = "Book updated successfully.";
	public const string UserUpdatedSuccessfully = "User updated successfully.";
	public const string AuthorUpdateFailed = "Failed to update author.";
	public const string TranslatorUpdateFailed = "Failed to update translator.";
	public const string BookUpdateFailed = "Failed to update book.";
	public const string UserUpdateFailed = "Failed to update user.";


	// Removing Messages
	public const string AuthorRemovedSuccessfully = "Author removed successfully.";
	public const string TranslatorRemovedSuccessfully = "Translator removed successfully.";
	public const string BookRemovedSuccessfully = "Book removed successfully.";
	public const string UserRemovedSuccessfully = "User removed successfully.";
	public const string AuthorRemoveFailed = "Failed to remove author.";
	public const string TranslatorRemoveFailed = "Failed to remove translator.";
	public const string BookRemoveFailed = "Failed to remove book.";
	public const string UserRemoveFailed = "Failed to remove user";


	// Searching Messages
	public const string NotAvailableAuthor = "No authors found. First add a new author.";
	public const string NotAuthorMatched = "No authors matched your search.";
	public const string NotAvailableBook = "No books found. First add a new book.";
	public const string NotBookMatched = "No books matched your search.";
	public const string NotAvailableUser = "No users found. First add a new user.";
	public const string NotUserMatched = "No users matched your search.";
	public const string UserNotFound = "User not found.";
	public const string NotRoleMatched = "No roles matched your search.";
	public const string NotAvailableLoan = "No loans found. First add a new loan."; // Need help
	public const string NotAvailableActionLoan = "No action loans found. First add a new loan.";
	public const string NotLoanMatched = "No loans matched your search.";


	// Duplicate Messages
	public const string FailureDuplicateAuthorByName = "An author with the same first and last name already exists.";
	public const string FailureDuplicateAuthorByNationalCode = "An author with the same national code already exists.";
	public const string FailureDuplicateAuthorByEmail = "An author with the same email already exists.";
	public const string FailureDuplicateAuthorByPhoneNumber = "An author with the same phone number already exists.";
	public const string FailureDuplicateTranslatorByName = "A translator with the same first and last name already exists.";
	public const string FailureDuplicateTranslatorByNationalCode = "A translator with the same national code already exists.";
	public const string FailureDuplicateTranslatorByEmail = "A translator with the same email already exists.";
	public const string FailureDuplicateTranslatorByPhoneNumber = "A translator with the same phone number already exists.";
	public const string FailureDuplicateBookByName = "A book with the same name already exists.";
	public const string FailureDuplicateBookByISBN = "A book with the same ISBN already exists.";
	public const string FailureDuplicateBookByAuthor = "This is already the book's author";
	public const string FailureDuplicateUserByName = "A user with the same first and last name already exists.";
	public const string FailureDuplicateUserByNationalCode = "A user with the same national code already exists.";
	public const string FailureDuplicateUserByEmail = "A user with the same email already exists.";
	public const string FailureDuplicateUserByPhoneNumber = "A user with the same phone number already exists.";
	public const string FailureDuplicateUserByRole = "This user has the same role.";
	public const string FailureDuplicateRolesSelected = "Duplicate roles selected.";
	public const string DuplicateRolesRemoved = "Duplicate roles were removed.";


	// Loan Messages
	public const string BorrowedSuccessfully = "Book borrowed successfully.";
	public const string ReturnedSuccessfully = "Book returned successfully.";
	public const string RenewedSuccessfully = "Loan renewed successfully.";
	public const string MaximumLoansReached = "Borrowing failed. The user has reached the maximum number of active loans.";
	public const string BookAlreadyBorrowed = "Borrowing failed. The user has already borrowed this book and must return it before borrowing another copy.";
	public const string ActiveLoanNotFound = "No active loan was found for the selected user and book.";
	public const string NoActiveLoans = "No active loans found.";
	public const string NotAvailableBook2Borrow = "No books are currently available to borrow";
	public const string UserHasNoBorrowedBooks = "This user has no borrowed books.";
	public const string NoOverdueLoans = "No overdue loans at the moment.";
	public const string MembershipExpired = "Membership expired. Please renew your membership to borrow books.";
	public const string NotEnoughCopiesAvailable = "Not enough copies available.";
	public const string NoLoanHistoryForUser = "This user has no loan history.";



	// Other Messages
	public const string EmptyInput = "Input cannot be empty. Please try again.";
	public const string WrongTotalCopies = "Total copies must be greater than zero.";
	public const string NoChangesDetected = "The new value is identical to the current one. No changes were made.";
	public const string Press2Continue = "\nPress any key to continue...";
	public const string NotAllowedMultipleRole = "Multiple roles are not allowed. Please enter only a single role ID.";
}