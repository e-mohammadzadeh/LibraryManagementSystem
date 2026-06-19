using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Helpers;

public static class Validator
{
	public static bool NationalCodeValidator(string nationalCode)
	{
		return !string.IsNullOrWhiteSpace(nationalCode) && Regex.IsMatch(nationalCode, @"^\d{10}$");
	}

	public static bool EmailValidator(string email)
	{
		return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
	}

	public static bool PhoneNumberValidator(string phoneNumber)
	{
		return !string.IsNullOrWhiteSpace(phoneNumber) && Regex.IsMatch(phoneNumber, @"^\d{11}$");
	}

	public static bool BirthDateValidator(DateOnly birthDate)
	{
		var today = DateOnly.FromDateTime(DateTime.Now);
		return birthDate <= today && birthDate >= today.AddYears(-120);
	}
}