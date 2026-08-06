namespace LibraryManagementSystem.Application.Common;

public static class Messages
{
	// 1. GENERAL MESSAGES (Cross‑cutting) ────────────────────────────────────────────────────────────
	public const string PressToContinue = "\nPress any key to continue...";
	public const string NoChangesDetected = "No changes detected. The new value is identical to the current one.";
	public const string EmptyInput = "Input cannot be empty. Please try again.";
	public const string InvalidInput = "Invalid input. Please try again.";
	public const string InvalidNumber = "Invalid number. Please try again.";
	public const string InvalidYesNo = "Invalid input. Please enter 'y' or 'n'.";
	public const string InvalidMenuChoice = "Invalid selection. Please try again.\n";
	public const string InvalidDateRange = "Invalid date. Please enter a date on or before today.";
	public const string InvalidDateFormat = "Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).";
	public const string DuplicateRemoved = "Duplicate items were removed.";
	public const string NotAllowedMultiSelections = "Multiple selections are not allowed. Please enter a single number.";




	// 2. VALIDATION MESSAGES (Format‑specific) ────────────────────────────────────────────────────────────
	public const string InvalidNationalCode = "Invalid national code. Please enter exactly 10 digits without dashes or spaces.";
	public const string InvalidEmail = "Invalid email address. Please enter a valid email (e.g., name@domain.com).";
	public const string InvalidPhoneNumber = "Invalid phone number. Please enter exactly 11 digits.";
	public const string InvalidBirthDate = "Invalid birth date. Please enter a past date that is no more than 120 years ago.";
	public const string InvalidCharacters = "The name can only contain letters, numbers, spaces, and basic punctuation.";
	public const string InvalidOptionSelection = "Please select at least one option.";



	// 3. Password Validation ────────────────────────────────────────────────────────────
	public const string MinimumPasswordLength = "Password must contain at least 8 characters.";
	public const string MaximumPasswordLength = "Password cannot contain more than 64 characters.";
	public const string OneUppercaseLetter = "Password must contain at least one uppercase letter.";
	public const string OneLowercaseLetter = "Password must contain at least one lowercase letter.";
	public const string PasswordContainDigit = "Password must contain at least one digit.";
	public const string PasswordContainSpecialCharacter = "Password must contain at least one special character.";
	public const string NotEmptyPassword = "Password cannot be empty.";
	public const string PasswordChangedSuccessfully = "Password changed successfully.";
	public const string PasswordChangeFailed = "Failed to change password. Please check your current password and try again.";
	public const string CanChangeOwnPassword = "You can only change your own password.";
	public const string PasswordMatchedFailed = "New password and confirmation do not match.";
	public const string SelectDifferentNewPassword = "New password must be different from the current password.";


	// 4. AUTHOR MESSAGES ────────────────────────────────────────────────────────────
	// Add
	public const string AuthorAddedSuccessfully = "Author added successfully.";
	public const string AuthorAddFailed = "Failed to add author.";
	public const string DuplicateAuthorNameWarning = "An author with the same name already exists (ID: {0}).";

	// Update
	public const string AuthorUpdatedSuccessfully = "Author updated successfully.";
	public const string AuthorUpdateFailed = "Failed to update author.";

	// Remove
	public const string AuthorRemovedSuccessfully = "Author removed successfully.";
	public const string AuthorRemoveFailed = "Failed to remove author.";
	public const string CannotRemoveLastAuthor = "Cannot remove the only author. A book must have at least one author.";
	public const string AuthorHasAssociatedBooks = "This author cannot be removed because they have associated books.";

	// Search
	public const string NotAvailableAuthor = "No authors found. Please add a new author first.";
	public const string NotAuthorMatched = "No authors matched your search.";
	public const string AuthorNotFoundFormat = "Author with ID {0} was not found.";

	// Duplicate
	public const string DuplicateAuthorsNotAllowed = "Duplicate authors are not allowed. Please enter each author only once.";
	public const string DuplicateAuthorsNotAllowedByName = "An author with the same first and last name already exists.";
	public const string DuplicateAuthorsNotAllowedByNationalCode = "An author with the same national code already exists.";
	public const string DuplicateAuthorsNotAllowedByEmail = "An author with the same email already exists.";
	public const string DuplicateAuthorsNotAllowedByPhoneNumber = "An author with the same phone number already exists.";




	// 5. TRANSLATOR MESSAGES ────────────────────────────────────────────────────────────
	// Add
	public const string TranslatorAddedSuccessfully = "Translator added successfully.";
	public const string TranslatorAddFailed = "Failed to add translator.";
	public const string AllTranslatorsAssigned = "All available translators are already assigned to this book.";
	public const string AddTranslatorInAdd = "\nNo translators found. Do you want to create a new translator now (Optional)";

	// Update
	public const string TranslatorUpdatedSuccessfully = "Translator updated successfully.";
	public const string TranslatorUpdateFailed = "Failed to update translator.";

	// Remove
	public const string TranslatorRemovedSuccessfully = "Translator removed successfully.";
	public const string TranslatorRemoveFailed = "Failed to remove translator.";
	public const string NoTranslatorToRemove = "This book has no translators to remove.";
	public const string RemoveAllTranslators = "Are you sure you want to remove ALL translators from this book";

	// Search
	public const string NotAvailableTranslator = "No translators found. Please add a new translator first.";
	public const string NotTranslatorMatched = "No translators matched your search.";
	public const string TranslatorNotFoundFormat = "Translator with ID {0} was not found.";

	// Duplicate
	public const string DuplicateTranslatorsNotAllowed = "Duplicate translators are not allowed. Please enter each translator only once.";
	public const string DuplicateTranslatorsNotAllowedByName = "A translator with the same first and last name already exists.";
	public const string DuplicateTranslatorsNotAllowedByNationalCode = "A translator with the same national code already exists.";
	public const string DuplicateTranslatorsNotAllowedByEmail = "A translator with the same email already exists.";
	public const string DuplicateTranslatorsNotAllowedByPhoneNumber = "A translator with the same phone number already exists.";



	// 6. BOOK MESSAGES ────────────────────────────────────────────────────────────
	// Add
	public const string BookAddedSuccessfully = "Book added successfully.";
	public const string BookAddFailed = "Failed to add book.";
	public const string NotEnoughAuthors = "All available authors are already assigned to this book.";
	public const string AuthorCreationFailed = "Failed to retrieve the newly created author.";
	public const string BookRequiresAtLeastOneAuthor = "A book must have at least one author.";

	// Update
	public const string BookUpdatedSuccessfully = "Book updated successfully.";
	public const string BookUpdateFailed = "Failed to update book.";
	public const string TotalCopiesUpdateInvalid = "Cannot update total copies because it would result in negative available copies.";

	// Remove
	public const string BookRemovedSuccessfully = "Book removed successfully.";
	public const string BookRemoveFailed = "Failed to remove book.";
	public const string BookRemoveFailedBorrowed = "Failed to remove book. It is currently borrowed by user(s) with ID: {0}";
	public const string BookRemoveConfirmation = "\nAre you sure you want to remove '{0}'";

	// Search
	public const string NotAvailableBook = "No books found. Please add a new book first.";
	public const string NotBookMatched = "No books matched your search.";

	// Duplicate
	public const string DuplicateBooksNotAllowedByName = "A book with the same name already exists.";
	public const string DuplicateBooksNotAllowedByISBN = "A book with the same ISBN already exists.";
	public const string AuthorAlreadyAssignedToBook = "This author is already associated with this book.";

	// Validation
	public const string InvalidBookName = "Invalid book name. Please enter a name between 3 and 100 characters.";
	public const string InvalidISBN = "Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.";
	public const string InvalidGenre = "Invalid genre. Please select a valid genre.";
	public const string WrongTotalCopies = "Total copies must be greater than zero.";
	public const string InvalidBookSelection = "That book ID is not in the available list. Please select from the list above.";


	// 7. USER (Member/Admin/Manager) MESSAGES ────────────────────────────────────────────────────────────
	// Add
	public const string UserAddedSuccessfully = "User added successfully.";
	public const string UserAddFailed = "Failed to add user.";

	// Update
	public const string UserUpdatedSuccessfully = "User updated successfully.";
	public const string UserUpdateFailed = "Failed to update user.";

	// Remove
	public const string UserRemovedSuccessfully = "User removed successfully.";
	public const string UserRemoveFailed = "Failed to remove user.";
	public const string CannotRemoveYourself = "You cannot remove your own account.";
	public const string UserRemovalFailedByActiveLoans = "This user cannot be removed because they have active loans.";
	public const string UserRemovalFailedByUnpaidFines = "This user cannot be removed because they have unpaid fines.";
	public const string UserAutoRemovedSuccessfully = "User has been automatically removed from the system after settling all obligations.";
	public const string UserEligibleForRemoval = "This user has been flagged for removal. They must pay all fines and return all books before their account is deleted.";

	// Search
	public const string NotAvailableUser = "No users found. Please add a new user first.";
	public const string NotUserMatched = "No users matched your search.";
	public const string UserNotFound = "User not found.";

	// Duplicate
	public const string DuplicateUsersNotAllowedByName = "A user with the same first and last name already exists.";
	public const string DuplicateUsersNotAllowedByNationalCode = "A user with the same national code already exists.";
	public const string DuplicateUsersNotAllowedByEmail = "A user with the same email already exists.";
	public const string DuplicateUsersNotAllowedByPhoneNumber = "A user with the same phone number already exists.";
	public const string DuplicateUsersNotAllowedByRole = "This user already has this role.";

	// Membership
	public const string MembershipExpired = "Membership expired. Please renew your membership.";
	public const string InactiveAccount = "This account is inactive.";

	// Role
	public const string FailureDuplicateRolesSelected = "Duplicate roles selected.";
	public const string NotRoleMatched = "No roles matched your search.";
	public const string SelectRolePrompt = "Select a role for this user:";




	// 8. LOAN MESSAGES ────────────────────────────────────────────────────────────
	// Borrow / Return / Renew
	public const string BorrowedSuccessfully = "Book borrowed successfully.";
	public const string ReturnedSuccessfully = "Book returned successfully.";
	public const string RenewedSuccessfully = "Loan renewed successfully.";
	public const string LoanNotYetReturned = "Loan has not been returned yet.";
	public const string FlaggedForRemoval = "This account has been flagged for removal due to reaching the maximum fine limit. Borrowing is disabled.";
	public const string BorrowFailedForFine = "You cannot borrow books while you have unpaid fines. Please pay your fines first.";

	// Search / Availability
	public const string NotAvailableLoan = "No loans found.";
	public const string NotAvailableBookToBorrow = "No books are currently available to borrow.";
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
	public const string UserHasUnpaidFines = "You cannot renew a loan while you have unpaid fines. Please pay your fines first.";

	// Copy Availability
	public const string NotEnoughCopiesAvailable = "Not enough copies available for this book.";






	// 9. FINE MESSAGES ────────────────────────────────────────────────────────────
	public const string FineCreatedSuccessfully = "Fine created successfully.";
	public const string FineNotFound = "Fine not found.";
	public const string UnpaidFineNotFound = "No unpaid fines found.";
	public const string FinePaidSuccessfully = "Fine paid successfully.";
	public const string FineWaivedSuccessfully = "Fine waived successfully.";
	public const string NoFine = "This loan was returned on time. No fine has been applied.";
	public const string FineAlreadyExists = "An unpaid fine already exists for this loan.";
	public const string CanPayOwnFine = "You can only pay your own fines";
	public const string AdminOnlyWaive = "Only Admin can waive fines.";



	// 10. Authentication & Authorization ────────────────────────────────────────────────────────────
	public const string LoginSuccess = "Login successful.";
	public const string LoginFailed = "Login failed.";
	public const string LogoutSuccess = "Logged out successfully.";
	public const string LoginInputRequired = "Email and password are required.";
	public const string InvalidLoginInput = "Invalid email or password.";
	public const string AccessDenied = "Access denied. You do not have permission to perform this action.";
	public const string SessionExpired = "Your session has expired. Please log in again.";
	public const string AuthenticationRequired = "Access denied. You must be logged in.";
	public const string AdminRoleRequired = "Access denied. Admin role required.";
	public const string LibrarianRoleRequired = "Access denied. Librarian role required.";
	public const string MemberRoleRequired = "Access denied. Member role required.";
	public const string AdminOrLibrarianRoleRequired = "Access denied. Admin or Librarian role required.";
	public const string LibraryStatisticsAccessDenied = "Access denied. Only Admin or Librarian can view library statistics.";
	public const string CannotBorrowBooks = "You are not eligible to borrow books. Please check your membership status and outstanding fines.";
	public const string NoUserLoggedIn = "No user is currently logged in.";
	public const string BorrowBookForYourself = "You can only borrow books for yourself.";
	public const string ReturnOwnLoans = "You can only return your own loans.";
	public const string RenewOwnLoans = "You can only renew your own loans.";
	public const string ViewOwnLoans = "You can only view your own loans.";
	public const string NotAuthorized = "You are not authorized to access this menu.\n";



	// 11. UI Prompts & Questions ────────────────────────────────────────────────────────────
	public const string MainMenuQuestion = "Please enter a number: ";
	public const string AuthorSelectionForRemove = "Select the author you want to remove";
	public const string EditMenuQuestion = "Enter the number of the field you wish to edit";
	public const string EditContinuesQuestion = "Do you want to edit another field";
	public const string SelectReplacementTranslators = "Select the new translator(s) for this book";
	public const string SearchMenuQuestion = "Select a search field by entering its number";
	public const string TranslatorSelectionForRemove = "Select the translator you want to remove";
	public const string SubMenuPrompt = "What do you want to do?";
	public const string EnterEmailPrompt = "Enter your email address";
	public const string EnterPasswordPrompt = "Enter your {0} password";
	public const string PasswordConfirmation = "Confirm new password";
	public const string FineIdForPay = "Enter Fine ID to pay";
	public const string FineIdForWaive = "Enter Fine ID to waive";
	public const string ConfirmToPay = "Are you sure you want to pay this fine";
	public const string ConfirmToWaive = "Are you sure you want to waive this fine";



	// 12. Navigation ────────────────────────────────────────────────────────────
	// Back
	public const string BackToMainMenu = "Backing to main menu...\n";

	// Exit
	public const string ExitingProgram = "Exiting Program...\n";

	// Search Cancelled
	public const string SearchCancelled = "Search cancelled. Returning to {0} Menu...";

	// Edit Cancelled
	public const string EditCancelled = "Edit cancelled. Returning to {0} Menu...";

}