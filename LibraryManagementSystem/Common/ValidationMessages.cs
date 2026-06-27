namespace LibraryManagementSystem.Common;

public static class ValidationMessages
{
	public const string InvalidNationalCode = "Invalid national code. Please enter exactly 10 digits without dashes or spaces.";
	public const string InvalidEmail = "Invalid email address. Please enter a valid email (e.g., name@domain.com).";
	public const string InvalidPhoneNumber = "Invalid phone number. Please enter exactly 11 digits.";
	public const string InvalidBirthDate = "Invalid birth date. Please enter a past date that is no more than 120 years ago.";
	public const string InvalidDate = "Invalid date.  Please enter a date on or before today.";
	public const string InvalidBirthDateFormat = "Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).";

	public const string InvalidISBN = "Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.";
	public const string InvalidNumber = "Invalid number. Please try again.";
	public const string InvalidYesNo = "Invalid input. Please enter 'y' or 'n'.";
	public const string InvalidMenuChoice = "Invalid selection, Try again.\n";

	public const string SuccessAdd = "Author added successfully.";
	public const string FailureAdd = "Failed to add author.";

	public const string SuccessUpdate = "Author updated successfully.";
	public const string FailureUpdate = "Failed to update author.";

	public const string SuccessRemove = "Author removed successfully.";
	public const string FailureRemove = "Failed to remove author.";

	public const string NotAvailableAuthor = "No authors found. First add new author.";
	public const string NotAvailableBook = "";
	public const string NotAvailableLoan = "";

	public const string FailureDuplicateAuthorByName = "An author with the same first and last name already exists.";
	public const string FailureDuplicateAuthorByNationalCode = "An author with the same national code already exists.";
	public const string FailureDuplicateAuthorByEmail = "An author with the same email already exists.";
	public const string FailureDuplicateAuthorByPhoneNumber = "An author with the same phone number already exists.";

	public const string EmptyInput = "Input cannot be empty. Please try again.";
}