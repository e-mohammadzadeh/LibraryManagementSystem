using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Domain;

public class Person
{
	protected Person(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate)
	{
		FirstName = firstName;
		LastName = lastName;

		NationalCode = ValidateNationalCode(nationalCode);
		Email = ValidateEmail(email);
		PhoneNumber = ValidatePhoneNumber(phoneNumber);
		BirthDate = ValidateBirthDate(birthDate);
	}

	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? BirthDate { get; set; }


	private static string ValidateNationalCode(string nationalCode)
	{
		if (!string.IsNullOrWhiteSpace(nationalCode) && Regex.IsMatch(nationalCode, @"^\d{10}$"))
		{
			return nationalCode;
		}
		throw new ArgumentException("Invalid national code.Please try again.");
	}

	private static string ValidateEmail(string email)
	{
		if (!string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")) {
			return email;
		}
		throw new ArgumentException("Invalid email.Please try again.");
	}

	private static string ValidatePhoneNumber(string phoneNumber)
	{
		if (!string.IsNullOrWhiteSpace(phoneNumber) && Regex.IsMatch(phoneNumber, @"^\d{11}$")) {
			return phoneNumber;
		}
		throw new ArgumentException("Invalid phone number.Please try again.");
	}

	private static DateOnly ValidateBirthDate(DateOnly birthDate)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.Now);
		if (today >= birthDate && today.AddYears(-120) <= birthDate) {
			return birthDate;
		}
		throw new ArgumentException("Invalid birth date.Please try again.");
	}
}