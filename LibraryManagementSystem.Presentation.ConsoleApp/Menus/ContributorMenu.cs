using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

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


	public static void EditContributor(ContributorDto contributor, string menuTitle, string menuIdPrefix,
		IMenuService menuService, Func<ContributorDto, Guid?, ServiceResult<ContributorDto>> updateContributor,
		Guid updatedBy)
	{
		while (true)
		{
			Console.Clear();
			ContributorPrinter.PrintDetails(contributor, menuTitle);

			var selectedItem = MenuHelper.DisplayMenu(menuTitle, menuService.GetChildren($"{menuIdPrefix}.edit"));
			switch (selectedItem)
			{
				case null:
					return;

				case var _ when selectedItem == $"{menuIdPrefix}.edit.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled,
						menuTitle.Replace(" MENU", "", StringComparison.OrdinalIgnoreCase)));
					return;
			}

			var updated = true;
			switch (selectedItem)
			{
				case var _ when selectedItem == $"{menuIdPrefix}.edit.firstname":
				{
					var value = ConsoleHelper.GetValidName(
						"\nEnter new first name",
						ValidationConstants.MinNameLength,
						ValidationConstants.MaxNameLength);

					if (value != null)
						contributor.FirstName = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.lastname":
				{
					var value = ConsoleHelper.GetValidName(
						"\nEnter new last name",
						ValidationConstants.MinNameLength,
						ValidationConstants.MaxNameLength);

					if (value != null)
						contributor.LastName = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.national-code":
				{
					var value = ConsoleHelper.GetValidNationalCode("\nEnter new national code");

					if (value != null)
						contributor.NationalCode = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.email":
				{
					var value = ConsoleHelper.GetValidEmail("\nEnter new email");

					if (value != null)
						contributor.Email = Email.Create(value);
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.phone-number":
				{
					var value = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");

					if (value != null)
						contributor.PhoneNumber = PhoneNumber.Create(value);
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.birthdate":
				{
					var value = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");

					if (value != null)
						contributor.BirthDate = value.Value;
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.biography":
				{
					var value = ConsoleHelper.ReadString("\nEnter new biography");

					if (value != null)
						contributor.Biography = value;
					else
						updated = false;

					break;
				}

				default:
				{
					updated = false;
					break;
				}
			}

			if (updated)
			{
				var result = updateContributor(contributor, updatedBy);
				ConsoleHelper.ShowResult(result);
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
		}
	}
}