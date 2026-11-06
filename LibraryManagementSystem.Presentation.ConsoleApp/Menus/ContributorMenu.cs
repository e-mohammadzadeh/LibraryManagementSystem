using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public abstract class ContributorMenu
{
	public static ContributorDto? PromptForContributorDto(string contributorType, bool hasBio = true)
	{
		var label = string.IsNullOrWhiteSpace(contributorType) ? "" : $"{contributorType}'s ";

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

		if (!hasBio)
			return new ContributorDto
			{
				FirstName = firstName,
				LastName = lastName,
				NationalCode = nationalCode,
				Email = Email.Create(email),
				PhoneNumber = PhoneNumber.Create(phoneNumber),
				BirthDate = birthDate.Value
			};

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional)", true);
		return new ContributorDto
		{
			FirstName = firstName,
			LastName = lastName,
			NationalCode = nationalCode,
			Email = Email.Create(email),
			PhoneNumber = PhoneNumber.Create(phoneNumber),
			BirthDate = birthDate.Value,
			Biography = biography
		};
	}
}