namespace LibraryManagementSystem.Common;

public class ValidationResult
{
	private ValidationResult(bool isValid, string? errorMessage)
	{
		IsValid = isValid;
		ErrorMessage = errorMessage;
	}


	public bool IsValid { get; }
	public string? ErrorMessage { get; }


	public static ValidationResult Success() => new(true, null);

	public static ValidationResult Fail(string message) => new(false, message);

	public static ValidationResult Warning(string message) => new(true, message);
}