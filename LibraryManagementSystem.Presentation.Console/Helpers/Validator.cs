using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Helpers;

public static class Validator
{
	public static ValidationResult NationalCodeValidator(string nationalCode)
	{
		// 1. Basic format check: non-empty, digits only, exact length
		if (string.IsNullOrWhiteSpace(nationalCode) ||
		    !Regex.IsMatch(nationalCode, $@"^\d{{{ValidationConstants.NationalCodeLength}}}$"))
			return ValidationResult.Fail(ValidationMessages.InvalidNationalCode);

		// 2. Reject codes where every digit is the same (e.g., "0000000000", "1111111111", ..., "9999999999")
		return nationalCode.Any(c => c != nationalCode[0])
			? ValidationResult.Success()
			: ValidationResult.Fail(ValidationMessages.InvalidNationalCode);
	}


	public static ValidationResult EmailValidator(string email)
	{
		if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return ValidationResult.Fail(ValidationMessages.InvalidEmail);

		return ValidationResult.Success();
	}


	public static ValidationResult PhoneNumberValidator(string phoneNumber)
	{
		if (string.IsNullOrWhiteSpace(phoneNumber) ||
		    !Regex.IsMatch(phoneNumber, $@"^\d{{{ValidationConstants.PhoneNumberLength}}}$"))
			return ValidationResult.Fail(ValidationMessages.InvalidPhoneNumber);

		return phoneNumber.Any(c => c != phoneNumber[0])
			? ValidationResult.Success()
			: ValidationResult.Fail(ValidationMessages.InvalidPhoneNumber);
	}


	public static ValidationResult BirthDateValidator(DateOnly birthDate)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		return birthDate <= today && birthDate >= today.AddYears(-120)
			? ValidationResult.Success()
			: ValidationResult.Fail(ValidationMessages.InvalidBirthDate);
	}


	public static ValidationResult ISBNValidator(string isbn)
	{
		var cleaned = isbn.Replace("-", "");

		switch (cleaned.Length)
		{
			// Check for ISBN-13 (must be exactly 13 digits)
			case 13 when Regex.IsMatch(cleaned, @"^\d{" + ValidationConstants.ISBN13Length + "}$"):
			// Check for ISBN-10 (must be 10 characters: 9 digits + optional 'X')
			case 10 when Regex.IsMatch(cleaned, @"^\d{9}[\dX]$"):
				return ValidationResult.Success();
			default:
				return ValidationResult.Fail(ValidationMessages.InvalidISBN);
		}
	}


	public static ValidationResult NameValidator(string name, int minLength, int maxLength)
	{
		if (name.Contains("  "))
			return ValidationResult.Fail("The name cannot contain consecutive spaces.");

		if (name.Length < minLength || name.Length > maxLength)
			return ValidationResult.Fail(ValidationMessages.InvalidBookName);

		return !Regex.IsMatch(name, @"^(?=.*\p{L})[\p{L}\p{N}\s\-'\.,:;!?&()]+$")
			? ValidationResult.Fail(ValidationMessages.InvalidCharacters)
			: ValidationResult.Success();
	}


	public static ValidationResult DateValidator(DateOnly? date)
	{
		if (date == null)
			return ValidationResult.Fail(ValidationMessages.InvalidInput);

		var today = DateOnly.FromDateTime(DateTime.Today);
		return date <= today && date >= today.AddYears(-1500)
			? ValidationResult.Success()
			: ValidationResult.Fail(ValidationMessages.InvalidDate);
	}
}