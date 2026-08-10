using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class TranslatorMenu
{
	public static void TranslatorMenuController(TranslatorManagementService translatorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.AddTranslator,
			    Permission.EditTranslator,
			    Permission.SearchTranslator,
			    Permission.ViewTranslatorDetails,
			    Permission.ViewAllTranslators))
		{
			return;
		}

		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			switch (TranslatorMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddTranslator, Messages.AccessDenied))
						break;
					AddTranslator(translatorManagementService, session);
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditTranslator, Messages.AccessDenied))
						break;
					EditTranslator(translatorManagementService, session);
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveTranslator, Messages.AccessDenied))
						break;
					RemoveTranslator(translatorManagementService, session);
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchTranslator, Messages.AccessDenied))
						break;
					SearchTranslator(translatorManagementService);
					break;
				}
				case 5:
				{

					if (!SessionGuard.RequirePermission(authorization, Permission.ViewTranslatorDetails, Messages.AccessDenied))
						break;
					var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
						MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);
					if (desiredTranslator is not null)
						TranslatorPrinter.PrintDetails(desiredTranslator);
					break;
				}
				case 6:
				{
					
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllTranslators, Messages.AccessDenied))
						break;
					if (translatorManagementService.GetAllTranslators().Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					else
						TranslatorPrinter.PrintTable(translatorManagementService.GetAllTranslators());
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
			ConsoleHelper.Pause();
		}
	}


	private static int TranslatorMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Add Translator", authorization.HasPermission(Permission.AddTranslator)),
			(2, "Edit Translator", authorization.HasPermission(Permission.EditTranslator)),
			(3, "Remove Translator", authorization.HasPermission(Permission.RemoveTranslator)),
			(4, "Search Translator", authorization.HasPermission(Permission.SearchTranslator)),
			(5, "View Translator Details", authorization.HasPermission(Permission.ViewTranslatorDetails)),
			(6, "View All Translators", authorization.HasPermission(Permission.ViewAllTranslators)),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 33) + " TRANSLATOR MENU " + new string('=', 33));

			var displayNumber = 1;
			foreach (var item in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {item.DisplayText}");
				displayNumber++;
			}

			Console.WriteLine(new string('=', 82));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count) return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
	}


	private static void AddTranslator(TranslatorManagementService translatorManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " ADDING TRANSLATOR MENU " + new string('=', 36));
		var translatorDto = PromptForTranslatorDto();
		if (translatorDto is null) return;

		var result = translatorManagementService.AddTranslator(translatorDto);
		ConsoleHelper.ShowResult(result);
	}


	public static CreateTranslatorDto? PromptForTranslatorDto()
	{
		var fields = PersonHelper.PromptForPersonFields("translator");
		if (fields is null) return null;

		return new CreateTranslatorDto
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = fields.Email, PhoneNumber = fields.PhoneNumber, BirthDate = fields.BirthDate
		};
	}


	private static void EditTranslator(TranslatorManagementService translatorManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " EDITING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);
		if (desiredTranslator is null) return;

		while (true)
		{
			Console.WriteLine("\n{0, -20} [{1}]", "1. First Name", desiredTranslator.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredTranslator.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredTranslator.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredTranslator.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredTranslator.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredTranslator.BirthDate);
			Console.WriteLine("7. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 7);
			if (editMenuChoice == null) return;

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
					ConsoleHelper.ShowError(string.Format(Messages.EditCancelled, "Translator"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static void RemoveTranslator(TranslatorManagementService translatorManagementService,
		ICurrentUserSession session)
	{
		if (!session.IsAdmin)
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		// TODO	(SQL Server)	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine(new string('=', 36) + " REMOVING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);

		PersonHelper.PerformRemove(desiredTranslator, desiredTranslator?.FirstName ?? "",
			desiredTranslator?.LastName ?? "", TranslatorPrinter.PrintDetails,
			() => translatorManagementService.RemoveTranslator(desiredTranslator!.Id));
	}


	private static void SearchTranslator(TranslatorManagementService translatorManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING TRANSLATOR MENU " + new string('=', 36));
			var translatorsList = translatorManagementService.GetAllTranslators();
			if (translatorsList.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
				ConsoleHelper.Pause();
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 5);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					PersonHelper.SearchAndDisplay("Enter a name to search",
						term => translatorManagementService.SearchTranslator(term, TranslatorSearchField.Name),
						TranslatorPrinter.PrintTable, Messages.NotTranslatorMatched);

					break;
				}
				case 2:
				{
					PersonHelper.SearchAndDisplay("Enter a national code to search",
						term => translatorManagementService.SearchTranslator(term, TranslatorSearchField.NationalCode),
						TranslatorPrinter.PrintTable, Messages.NotTranslatorMatched);

					break;
				}
				case 3:
				{
					PersonHelper.SearchAndDisplay("Enter an email to search",
						term => translatorManagementService.SearchTranslator(term, TranslatorSearchField.Email),
						TranslatorPrinter.PrintTable, Messages.NotTranslatorMatched);

					break;
				}
				case 4:
				{
					PersonHelper.SearchAndDisplay("Enter a phone number to search",
						term => translatorManagementService.SearchTranslator(term, TranslatorSearchField.PhoneNumber),
						TranslatorPrinter.PrintTable, Messages.NotTranslatorMatched);

					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Translator"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}



	private static void PerformUpdate<T>(TranslatorManagementService translatorManagementService,
		int desiredTranslatorId,
		T? newValue,
		Func<T, UpdateTranslatorDto> buildDto)
	{
		if (newValue is null) return;

		var dto = buildDto(newValue);
		var result = translatorManagementService.UpdateTranslator(desiredTranslatorId, dto);
		ConsoleHelper.ShowResult(result);
	}
}