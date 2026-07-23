namespace LibraryManagementSystem.Application.Common;

public static class ValidationMessages
{
	// 1. GENERAL MESSAGES (Cross‑cutting)
	public const string Press2Continue = "\nPress any key to continue...";
	public const string NoChangesDetected = "No changes detected. The new value is identical to the current one.";
	public const string EmptyInput = "Input cannot be empty. Please try again.";
	public const string InvalidInput = "Invalid input. Please try again.";
	public const string InvalidNumber = "Invalid number. Please try again.";
	public const string InvalidYesNo = "Invalid input. Please enter 'y' or 'n'.";
	public const string InvalidMenuChoice = "Invalid selection. Please try again.\n";
	public const string InvalidDate = "Invalid date. Please enter a date on or before today.";
	public const string InvalidBirthDateFormat = "Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).";
	public const string NotAllowedMultipleRole = "Multiple roles are not allowed. Please select a single role ID.";



	// 2. VALIDATION MESSAGES (Format‑specific)
	public const string InvalidNationalCode = "Invalid national code. Please enter exactly 10 digits without dashes or spaces.";
	public const string InvalidEmail = "Invalid email address. Please enter a valid email (e.g., name@domain.com).";
	public const string InvalidPhoneNumber = "Invalid phone number. Please enter exactly 11 digits.";
	public const string InvalidBirthDate = "Invalid birth date. Please enter a past date that is no more than 120 years ago.";
	public const string InvalidCharacters = "The name can only contain letters, numbers, spaces, and basic punctuation.";
	public const string InvalidRoleSelection = "Please select at least one role number.";
	public const string GetRole = "Select a role for this user:";



	// 3. AUTHOR MESSAGES
	// Add
	public const string AuthorAddedSuccessfully = "Author added successfully.";
	public const string AuthorAddFailed = "Failed to add author.";

	// Update
	public const string AuthorUpdatedSuccessfully = "Author updated successfully.";
	public const string AuthorUpdateFailed = "Failed to update author.";

	// Remove
	public const string AuthorRemovedSuccessfully = "Author removed successfully.";
	public const string AuthorRemoveFailed = "Failed to remove author.";

	// Search
	public const string NotAvailableAuthor = "No authors found. Please add a new author first.";
	public const string NotAuthorMatched = "No authors matched your search.";
	public const string AuthorNotFoundFormat = "Author with ID {0} was not found.";

	// Duplicate
	public const string FailureDuplicateAuthor = "Duplicate authors are not allowed. Please enter each author only once.";
	public const string FailureDuplicateAuthorByName = "An author with the same first and last name already exists.";
	public const string FailureDuplicateAuthorByNationalCode = "An author with the same national code already exists.";
	public const string FailureDuplicateAuthorByEmail = "An author with the same email already exists.";
	public const string FailureDuplicateAuthorByPhoneNumber = "An author with the same phone number already exists.";
	public const string DuplicateAuthorsRemoved = "Duplicate authors were removed.";




	// 4. TRANSLATOR MESSAGES
	// Add
	public const string TranslatorAddedSuccessfully = "Translator added successfully.";
	public const string TranslatorAddFailed = "Failed to add translator.";

	// Update
	public const string TranslatorUpdatedSuccessfully = "Translator updated successfully.";
	public const string TranslatorUpdateFailed = "Failed to update translator.";

	// Remove
	public const string TranslatorRemovedSuccessfully = "Translator removed successfully.";
	public const string TranslatorRemoveFailed = "Failed to remove translator.";

	// Search
	public const string NotAvailableTranslator = "No translators found. Please add a new translator first.";
	public const string NotTranslatorMatched = "No translators matched your search.";
	public const string TranslatorNotFoundFormat = "Translator with ID {0} was not found.";


	// Duplicate
	public const string FailureDuplicateTranslator = "Duplicate translators are not allowed. Please enter each translator only once.";
	public const string FailureDuplicateTranslatorByName = "A translator with the same first and last name already exists.";
	public const string FailureDuplicateTranslatorByNationalCode = "A translator with the same national code already exists.";
	public const string FailureDuplicateTranslatorByEmail = "A translator with the same email already exists.";
	public const string FailureDuplicateTranslatorByPhoneNumber = "A translator with the same phone number already exists.";



	// 5. BOOK MESSAGES
	// Add
	public const string BookAddedSuccessfully = "Book added successfully.";
	public const string BookAddFailed = "Failed to add book.";

	// Update
	public const string BookUpdatedSuccessfully = "Book updated successfully.";
	public const string BookUpdateFailed = "Failed to update book.";
	public const string TotalCopiesUpdateInvalid = "Cannot update total copies because it would result in negative available copies.";

	// Remove
	public const string BookRemovedSuccessfully = "Book removed successfully.";
	public const string BookRemoveFailed = "Failed to remove book.";
	public const string BookRemoveFailedBorrowed = "Failed to remove Book. It is currently borrowed by user(s).";

	// Search
	public const string NotAvailableBook = "No books found. Please add a new book first.";
	public const string NotBookMatched = "No books matched your search.";

	// Duplicate
	public const string FailureDuplicateBookByName = "A book with the same name already exists.";
	public const string FailureDuplicateBookByISBN = "A book with the same ISBN already exists.";
	public const string FailureDuplicateBookByAuthor = "This author is already associated with this book.";

	// Validation
	public const string InvalidBookName = "Invalid book name. Please enter a name between 3 and 100 characters.";
	public const string InvalidISBN = "Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.";
	public const string InvalidGenre = "Invalid genre. Please select a valid genre.";
	public const string WrongTotalCopies = "Total copies must be greater than zero.";
	public const string BookRequiresAtLeastOneAuthor = "A book must have at least one author.";



	// 6. USER (Member/Admin/Manager) MESSAGES
	// Add
	public const string UserAddedSuccessfully = "User added successfully.";
	public const string UserAddFailed = "Failed to add user.";

	// Update
	public const string UserUpdatedSuccessfully = "User updated successfully.";
	public const string UserUpdateFailed = "Failed to update user.";

	// Remove
	public const string UserRemovedSuccessfully = "User removed successfully.";
	public const string UserRemoveFailed = "Failed to remove user.";

	// Search
	public const string NotAvailableUser = "No users found. Please add a new user first.";
	public const string NotUserMatched = "No users matched your search.";
	public const string UserNotFound = "User not found.";

	// Duplicate
	public const string FailureDuplicateUserByName = "A user with the same first and last name already exists.";
	public const string FailureDuplicateUserByNationalCode = "A user with the same national code already exists.";
	public const string FailureDuplicateUserByEmail = "A user with the same email already exists.";
	public const string FailureDuplicateUserByPhoneNumber = "A user with the same phone number already exists.";
	public const string FailureDuplicateUserByRole = "This user already has this role.";
	public const string FailureDuplicateRolesSelected = "Duplicate roles selected.";
	public const string DuplicateRolesRemoved = "Duplicate roles were removed.";

	// Membership
	public const string MembershipExpired = "Membership expired. Please renew your membership to borrow books.";



	// 7. LOAN MESSAGES
	// Borrow / Return / Renew
	public const string BorrowedSuccessfully = "Book borrowed successfully.";
	public const string ReturnedSuccessfully = "Book returned successfully.";
	public const string RenewedSuccessfully = "Loan renewed successfully.";

	// Search / Availability
	public const string NotAvailableLoan = "No loans found. Please add a new loan first.";
	public const string NotAvailableBook2Borrow = "No books are currently available to borrow.";
	public const string NotLoanMatched = "No loans matched your search.";
	public const string NoActiveLoans = "No active loans found.";
	public const string NoOverdueLoans = "No overdue loans at the moment.";
	public const string NotAvailableActionLoan = "No actionable loans found.";

	// User‑specific Loan Errors
	public const string MaximumLoansReached = "Borrowing failed. This user has reached the maximum number of active loans.";
	public const string BookAlreadyBorrowed = "This user has already borrowed this book. Please return it before borrowing another copy.";
	public const string ActiveLoanNotFound = "No active loan found for the selected user and book.";
	public const string UserHasNoBorrowedBooks = "This user has no borrowed books.";
	public const string NoLoanHistoryForUser = "This user has no loan history.";

	// Copy Availability
	public const string NotEnoughCopiesAvailable = "Not enough copies available for this book.";



	// 8. ROLE MESSAGES
	public const string NotRoleMatched = "No roles matched your search.";
}