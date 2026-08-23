using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translators;
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
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (TranslatorMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddTranslator, Messages.AccessDenied))
						break;
					Console.Clear();
					AddTranslator(translatorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditTranslator,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					EditTranslator(translatorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveTranslator,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveTranslator(translatorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchTranslator,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					SearchTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewTranslatorDetails,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
						MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);
					if (desiredTranslator is not null) TranslatorPrinter.PrintDetails(desiredTranslator);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					ViewBooksByTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllTranslators,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					if (translatorManagementService.GetAllTranslators().Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					else
						TranslatorPrinter.PrintFullTable(translatorManagementService.GetAllTranslators());
					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
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
			(6, "View Translator's Books", authorization.HasPermission(Permission.ViewTranslatorBooks)),
			(7, "View All Translators", authorization.HasPermission(Permission.ViewAllTranslators)),
			(8, "Back", true)
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


	private static void AddTranslator(TranslatorManagementService translatorManagementService)
	{
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


	private static void EditTranslator(TranslatorManagementService translatorManagementService)
	{
		Console.WriteLine(new string('=', 36) + " EDITING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);
		if (desiredTranslator is null) return;

		while (true)
		{
			Console.Clear();
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
					Console.Clear();
					var translatorNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewFirstName, v => new UpdateTranslatorDto { FirstName = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 2:
				{
					Console.Clear();
					var translatorNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewLastName, v => new UpdateTranslatorDto { LastName = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 3:
				{
					Console.Clear();
					var translatorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewNationalCode, v => new UpdateTranslatorDto { NationalCode = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 4:
				{
					Console.Clear();
					var translatorNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewEmail,
						v => new UpdateTranslatorDto { Email = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 5:
				{
					Console.Clear();
					var translatorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewPhoneNumber, v => new UpdateTranslatorDto { PhoneNumber = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 6:
				{
					Console.Clear();
					var translatorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewBirthDate, v => new UpdateTranslatorDto { BirthDate = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "Translator"));
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
		}
	}


	private static void RemoveTranslator(TranslatorManagementService translatorManagementService)
	{
		// TODO	(SQL Server)	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine(new string('=', 36) + " REMOVING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);

		PersonHelper.PerformRemove(desiredTranslator, desiredTranslator?.FirstName ?? "",
			desiredTranslator?.LastName ?? "", TranslatorPrinter.PrintDetails,
			() => translatorManagementService.RemoveTranslator(desiredTranslator!.Id));
	}


	private static void SearchTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasAnyPermission(Permission.SearchTranslator, Permission.SearchTranslatorByName,
			    Permission.SearchTranslatorByNationalCode, Permission.SearchTranslatorByEmail,
			    Permission.SearchTranslatorByPhoneNumber))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Action<IReadOnlyList<TranslatorDto>> printer =
			authorization.HasAnyPermission(Permission.ViewTranslatorDetails, Permission.ViewAllTranslators)
				? TranslatorPrinter.PrintFullTable
				: TranslatorPrinter.PrintTable;

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING TRANSLATOR MENU " + new string('=', 36));

			if (translatorManagementService.GetAllTranslators().Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
				return;
			}

			var items = new List<(TranslatorSearchField? Field, string Label, Permission Permission)>
			{
				(TranslatorSearchField.Name, "Name", Permission.SearchTranslatorByName),
				(TranslatorSearchField.NationalCode, "National Code", Permission.SearchTranslatorByNationalCode),
				(TranslatorSearchField.Email, "Email", Permission.SearchTranslatorByEmail),
				(TranslatorSearchField.PhoneNumber, "Phone Number", Permission.SearchTranslatorByPhoneNumber),
			};

			var available = items.Where(i => authorization.HasPermission(i.Permission)).ToList();

			var displayNumber = 1;
			foreach (var item in available) Console.WriteLine($"{displayNumber++}. {item.Label}");
			Console.WriteLine($"{displayNumber}. Back");

			var choice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, displayNumber);
			if (choice is null) return;

			if (choice == displayNumber) // Back
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Translator"));
				return;
			}

			var selected = available[choice.Value - 1];

			var prompt = selected.Field switch
			{
				TranslatorSearchField.Name => Messages.SearchName,
				TranslatorSearchField.NationalCode => Messages.SearchNationalCode,
				TranslatorSearchField.Email => Messages.SearchEmail,
				TranslatorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			PersonHelper.SearchAndDisplay(prompt,
				term => translatorManagementService.SearchTranslator(term, selected.Field!.Value), printer,
				Messages.NotTranslatorMatched);

			ConsoleHelper.Pause();
		}
	}


	private static TranslatorDto? PerformUpdate<T>(TranslatorManagementService translatorManagementService,
		int desiredTranslatorId, T? newValue, Func<T, UpdateTranslatorDto> buildDto)
	{
		if (newValue is null) return null;

		var dto = buildDto(newValue);
		var result = translatorManagementService.UpdateTranslator(desiredTranslatorId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Data;
	}


	private static void ViewBooksByTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!SessionGuard.RequirePermission(authorization, Permission.ViewTranslatorBooks, Messages.AccessDenied))
			return;
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			MenuHelper.SelectTranslator, Messages.NotAvailableTranslator);
		if (desiredTranslator is null) return;

		var books = translatorManagementService.GetBooksByTranslator(desiredTranslator.Id);
		if (books.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.TranslatorHasNoBooks);
			return;
		}

		BookPrinter.PrintTable(books, authorization);
	}
}