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
		NationalCode = nationalCode;
		Email = email;
		PhoneNumber = phoneNumber;
		BirthDate = birthDate;
	}


	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string NationalCode { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public DateOnly BirthDate { get; set; }
}