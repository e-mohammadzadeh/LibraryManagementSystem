using System.Text.RegularExpressions;
using LibraryManagementSystem.Common;

namespace LibraryManagementSystem.Helpers;

public static class Validator
{
	public static bool NationalCodeValidator(string nationalCode)
	{
		// 1. Basic format check: non-empty, digits only, exact length
		if (string.IsNullOrWhiteSpace(nationalCode) ||
		    !Regex.IsMatch(nationalCode, $@"^\d{{{ValidationConstants.NationalCodeLength}}}$"))
			return false;

		// 2. Reject codes where every digit is the same (e.g., "0000000000", "1111111111", ..., "9999999999")
		return nationalCode.Any(c => c != nationalCode[0]);
	}


	public static bool EmailValidator(string email)
	{
		return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
	}


	public static bool PhoneNumberValidator(string phoneNumber)
	{
		if (string.IsNullOrWhiteSpace(phoneNumber) ||
		    !Regex.IsMatch(phoneNumber, $@"^\d{{{ValidationConstants.PhoneNumberLength}}}$"))
			return false;

		return phoneNumber.Any(c => c != phoneNumber[0]);
	}


	public static bool BirthDateValidator(DateOnly birthDate)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		return birthDate <= today && birthDate >= today.AddYears(-120);
	}


	public static bool ISBNValidator(string isbn)
	{
		var cleaned = isbn.Replace("-", "");

		switch (cleaned.Length)
		{
			// Check for ISBN-13 (must be exactly 13 digits)
			case 13 when Regex.IsMatch(cleaned, @"^\d{" + ValidationConstants.ISBN13Length + "}$"):
			// Check for ISBN-10 (must be 10 characters: 9 digits + optional 'X')
			case 10 when Regex.IsMatch(cleaned, @"^\d{9}[\dX]$"):
				return true;
			default:
				return false;
		}
	}


	public static bool NameValidator(string name, int minLength, int maxLength)
	{
		if (name.Contains("  "))
			return false;

		if (name.Length < minLength || name.Length > maxLength)
			return false;

		return Regex.IsMatch(name, @"^(?=.*\p{L})[\p{L}\p{N}\s\-'\.,:;!?&()]+$");
	}


	public static bool DateValidator(DateOnly? date)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		return date <= today && date >= today.AddYears(-1500);
	}
}