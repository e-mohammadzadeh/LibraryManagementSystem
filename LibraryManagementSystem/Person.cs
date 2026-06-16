using System.Text.RegularExpressions;

namespace LibraryManagementSystem;

public abstract class Person
{
	protected Person(string firstName, string lastName, string nationalCode, string email, string phoneNumber, DateOnly birthDate)
	{
		FirstName = firstName;
		LastName = lastName;

		if (!string.IsNullOrWhiteSpace(nationalCode) && Regex.IsMatch(nationalCode, @"^\d{6}$"))
		{
			NationalCode = nationalCode;
		}

		if (!string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
		{
			Email = email;
		}

		if (!string.IsNullOrWhiteSpace(phoneNumber) && Regex.IsMatch(phoneNumber, @"^\d{11}$"))
		{
			PhoneNumber = phoneNumber;
		}

		DateOnly today = DateOnly.FromDateTime(DateTime.Now);
		if (today >= birthDate && today.AddYears(-120) <= birthDate)
		{
			BirthDate = birthDate;
		}

	}
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? BirthDate { get; set; }
}