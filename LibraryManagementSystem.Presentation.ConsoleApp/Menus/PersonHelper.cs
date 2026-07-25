using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

internal static class PersonHelper
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


	internal static void SearchAndDisplay<TEntity>(string prompt, Func<string, IReadOnlyList<TEntity>> searchFn,
		Action<IReadOnlyList<TEntity>> printFn, string noResultsMessage)
	{
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem is null) return;

		var result = searchFn(searchItem);
		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(noResultsMessage);
			return;
		}

		printFn(result);
	}



	internal static void PerformRemove<TEntity>(TEntity? entity, string firstName, string lastName,
		Action<TEntity> printDetailsFn, Func<ServiceResult<TEntity>> removeFn) where TEntity : class
	{
		if (entity is null)
		{
			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
			return;
		}

		printDetailsFn(entity);
		var choice = ConsoleHelper.ReadYesNo($"Are you sure you want to remove {firstName} {lastName}");

		if (choice != true) return;

		var result = removeFn();
		ConsoleHelper.ShowResult(result);
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}
}