namespace LibraryManagementSystem.Common;

public static class ValidationMessages
{
	public const string InvalidPhoneNumber = "Invalid phone number. Please try again.";

	public const string InvalidEmail = "Invalid email address. Please try again.";

	public const string InvalidNationalCode = "Invalid national code. Please try again.";

	public const string InvalidBirthDate = "Invalid birth date. Please try again.";

	public const string InvalidBirthDateFormat = "Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).";

	public const string InvalidISBN = "Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.";

	public const string EmptyInput = "Input cannot be empty. Please try again.";

	public const string InvalidNumber = "Invalid number. Please try again.";

	public const string InvalidYesNo = "Invalid input. Please enter 'y' or 'n'.";

	public const string SuccessUpdate = "Author updated successfully.";

	public const string FailureUpdate = "Failed to update author.";

	public const string SuccessRemove = "Author removed successfully.";

	public const string FailureRemove = "Failed to remove author.";

	public const string NotAvailableAuthor = "No authors found. First add new author.";
	public const string NotAvailableBook = "";
	public const string NotAvailableLoan = "";
}