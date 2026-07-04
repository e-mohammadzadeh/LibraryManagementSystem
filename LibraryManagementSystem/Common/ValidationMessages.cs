namespace LibraryManagementSystem.Common;

public static class ValidationMessages
{
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


	// Adding Messages
	public const string AuthorAddedSuccessfully = "Author added successfully.";
	public const string BookAddedSuccessfully = "Book added successfully.";
	public const string AuthorAddFailed = "Failed to add author.";
	public const string BookAddFailed = "Failed to add book.";
	public const string MemberAddedSuccessfully = "Member added successfully.";
	public const string MemberAddFailed = "Failed to add member.";


	// Updating Messages
	public const string AuthorUpdatedSuccessfully = "Author updated successfully.";
	public const string BookUpdatedSuccessfully = "Book updated successfully.";
	public const string MemberUpdatedSuccessfully = "Member updated successfully.";
	public const string AuthorUpdateFailed = "Failed to update author.";
	public const string BookUpdateFailed = "Failed to update book.";
	public const string MemberUpdateFailed = "Failed to update member.";


	// Removing Messages
	public const string AuthorRemovedSuccessfully = "Author removed successfully.";
	public const string BookRemovedSuccessfully = "Book removed successfully.";
	public const string AuthorRemoveFailed = "Failed to remove author.";
	public const string BookRemoveFailed = "Failed to remove book.";


	// Searching Messages
	public const string NotAvailableAuthor = "No authors found. First add a new author.";
	public const string NotAuthorMatched = "No authors matched your search.";
	public const string NotAvailableBook = "No books found. First add a new book.";
	public const string NotBookMatched = "No books matched your search.";
	public const string NotAvailableLoan = "No loans found. First add a new loan."; // Need help
	public const string NotAvailableMember = "No members found. First add a new member.";


	// Duplicate Messages
	public const string FailureDuplicateAuthorByName = "An author with the same first and last name already exists.";
	public const string FailureDuplicateAuthorByNationalCode = "An author with the same national code already exists.";
	public const string FailureDuplicateAuthorByEmail = "An author with the same email already exists.";
	public const string FailureDuplicateAuthorByPhoneNumber = "An author with the same phone number already exists.";
	public const string FailureDuplicateBookByName = "A book with the same name already exists.";
	public const string FailureDuplicateBookByISBN = "A book with the same ISBN already exists.";
	public const string FailureDuplicateBookByAuthor = "This is already the book's author";
	public const string FailureDuplicateMemberByName = "A member with the same first and last name already exists.";
	public const string FailureDuplicateMemberByNationalCode = "A member with the same national code already exists.";
	public const string FailureDuplicateMemberByEmail = "A member with the same email already exists.";
	public const string FailureDuplicateMemberByPhoneNumber = "A member with the same phone number already exists.";


	// Other Messages
	public const string EmptyInput = "Input cannot be empty. Please try again.";
	public const string WrongTotalCopies = "Total copies must be greater than zero.";
	public const string NoChangesDetected = "The new value is identical to the current one. No changes were made.";
	public const string Press2Continue = "\nPress any key to continue...";
}