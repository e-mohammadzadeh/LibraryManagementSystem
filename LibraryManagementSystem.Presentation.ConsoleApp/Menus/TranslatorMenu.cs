using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class TranslatorMenu {
	public static void TranslatorMenuController(TranslatorManagementService translatorManagementService, LibraryStatisticsService statisticsService) {
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (TranslatorMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddTranslator(translatorManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					EditTranslator(translatorManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveTranslator(translatorManagementService);
					break;
				}
				case 4:
				{
					SearchTranslator(translatorManagementService);
					break;
				}
				case 5:
				{
					Console.Clear();
					var desiredTranslator = SelectExistingTranslator(translatorManagementService);
					if (desiredTranslator is not null)
					{
						TranslatorPrinter.PrintDetails(desiredTranslator);
						ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
						Console.ReadKey(true);
					}

					break;
				}
				case 6:
				{
					Console.Clear();
					if (translatorManagementService.GetAllTranslators().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableTranslator);
					else
						TranslatorPrinter.PrintTable(translatorManagementService.GetAllTranslators());

					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int TranslatorMenuList() {
		while (true)
		{
			Console.WriteLine("=========================== TRANSLATOR MENU ===========================");
			Console.WriteLine("1. Add Translator");
			Console.WriteLine("2. Edit Translator");
			Console.WriteLine("3. Remove Translator");
			Console.WriteLine("4. Search Translator");
			Console.WriteLine("5. View Translator Details");
			Console.WriteLine("6. View All Translators");
			Console.WriteLine("7. Back");
			Console.WriteLine("===================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7)
				return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	public static CreateTranslatorDto? PromptForTranslatorDto() {
		var firstName = ConsoleHelper.GetValidName("Enter translator's first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (firstName == null)
			return null;

		var lastName = ConsoleHelper.GetValidName("Enter translator's last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (lastName == null)
			return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode("Enter translator's national code");
		if (nationalCode == null)
			return null;

		var email = ConsoleHelper.GetValidEmail("Enter translator's email");
		if (email == null)
			return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter translator's phone number");
		if (phoneNumber == null)
			return null;

		var birthDate = ConsoleHelper.GetValidBirthDate("Enter translator's birth date");
		if (birthDate == null)
			return null;

		return new CreateTranslatorDto
		{
			FirstName = firstName,
			LastName = lastName,
			NationalCode = nationalCode,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = birthDate.Value
		};
	}


	private static void AddTranslator(TranslatorManagementService translatorManagementService) {
		Console.WriteLine("============================ ADDING TRANSLATOR MENU ============================");
		var translatorDto = PromptForTranslatorDto();
		if (translatorDto is null)
			return;

		var result = translatorManagementService.AddTranslator(translatorDto);
		ConsoleHelper.ShowResult(result);
	}


	private static void EditTranslator(TranslatorManagementService translatorManagementService) {
		Console.WriteLine("============================ EDITING AUTHOR MENU ============================");
		var desiredTranslator = SelectExistingTranslator(translatorManagementService);
		if (desiredTranslator is null)
			return;

		while (true)
		{
			Console.WriteLine("\n{0, -20} [{1}]", "1. First Name", desiredTranslator.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredTranslator.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredTranslator.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredTranslator.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredTranslator.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredTranslator.BirthDate);
			Console.WriteLine("7. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt("Enter the number of the field you wish to edit", 1, 7);
			if (editMenuChoice == null)
				return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var translatorNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewFirstName,
						v => new UpdateTranslatorDto { FirstName = v });

					break;
				}
				case 2:
				{
					var translatorNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewLastName,
						v => new UpdateTranslatorDto { LastName = v });

					break;
				}
				case 3:
				{
					var translatorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewNationalCode,
						v => new UpdateTranslatorDto { NationalCode = v });

					break;
				}
				case 4:
				{
					var translatorNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewEmail,
						v => new UpdateTranslatorDto { Email = v });

					break;
				}
				case 5:
				{
					var translatorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewPhoneNumber,
						v => new UpdateTranslatorDto { PhoneNumber = v });

					break;
				}
				case 6:
				{
					var translatorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewBirthDate,
						v => new UpdateTranslatorDto { BirthDate = v });

					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Edit cancelled. Returning to Translator Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo("Do you want to edit another field");
			if (choice != true)
				return;
			Console.Clear();
		}
	}


	private static void RemoveTranslator(TranslatorManagementService translatorManagementService) {
		// TODO	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine("============================ REMOVING TRANSLATOR MENU ============================");
		var desiredTranslator = SelectExistingTranslator(translatorManagementService);
		if (desiredTranslator == null)
		{
			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
			return;
		}

		TranslatorPrinter.PrintDetails(desiredTranslator);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredTranslator.FirstName} {desiredTranslator.LastName}");

		if (choice != true)
			return;
		var result = translatorManagementService.RemoveTranslator(desiredTranslator.Id);
		ConsoleHelper.ShowResult(result);
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}


	private static void SearchTranslator(TranslatorManagementService translatorManagementService) {
		while (true)
		{
			Console.Clear();
			Console.WriteLine("============================ SEARCHING TRANSLATOR MENU ============================");
			var translatorsList = translatorManagementService.GetAllTranslators();
			if (translatorsList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableTranslator);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 5);
			if (searchMenuChoice is null)
				return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchTranslatorsAndDisplay(translatorManagementService, "Enter a name to search",
						translator => $"{translator.FirstName} {translator.LastName}");

					break;
				}
				case 2:
				{
					SearchTranslatorsAndDisplay(translatorManagementService, "Enter a national code to search",
						translator => translator.NationalCode);

					break;
				}
				case 3:
				{
					SearchTranslatorsAndDisplay(translatorManagementService, "Enter an email to search",
						translator => translator.Email);

					break;
				}
				case 4:
				{
					SearchTranslatorsAndDisplay(translatorManagementService, "Enter a phone number to search",
						translator => translator.PhoneNumber);

					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo("Search cancelled. Returning to Translator Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
		}
	}



	private static void PerformUpdate<T>(TranslatorManagementService translatorManagementService, int desiredTranslatorId,
		T? newValue,
		Func<T, UpdateTranslatorDto> buildDto) {
		if (newValue is null)
			return;

		var dto = buildDto(newValue);
		var result = translatorManagementService.UpdateTranslator(desiredTranslatorId, dto);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchTranslatorsAndDisplay(TranslatorManagementService translatorManagementService, string prompt,
		Func<Translator, string?> selector) {
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem == null)
			return;

		var result = translatorManagementService.SearchTranslator(searchItem, selector);

		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotTranslatorMatched);
			return;
		}

		TranslatorPrinter.PrintTable(result);
	}


	private static Translator? SelectExistingTranslator(TranslatorManagementService translatorManagementService) {
		var translators = translatorManagementService.GetAllTranslators();
		if (translators.Count is not 0)
			return MenuHelper.SelectTranslator(translators);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableTranslator);
		return null;
	}
}