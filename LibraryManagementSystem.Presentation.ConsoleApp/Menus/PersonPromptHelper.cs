using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

internal static class PersonPromptHelper
{
	internal static PersonFieldsResult? PromptForPersonFields(string entityLabel = "")
	{
		var label = string.IsNullOrWhiteSpace(entityLabel) ? "" : $"{entityLabel}'s ";

		var firstName = ConsoleHelper.GetValidName($"Enter {label}first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);
		if (firstName is null) return null;

		var lastName = ConsoleHelper.GetValidName($"Enter {label}last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);
		if (lastName is null) return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode($"Enter {label}national code");
		if (nationalCode is null) return null;

		var email = ConsoleHelper.GetValidEmail($"Enter {label}email");
		if (email is null) return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter phone number");
		if (phoneNumber is null) return null;

		var birthDate = ConsoleHelper.GetValidBirthDate($"Enter {label}birth date");
		if (birthDate is null) return null;

		return new PersonFieldsResult(firstName, lastName, nationalCode, email, phoneNumber, birthDate.Value);
	}
}