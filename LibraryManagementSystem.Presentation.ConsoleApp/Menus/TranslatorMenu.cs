using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Search;
using LibraryManagementSystem.Domain.Enums.Sort;
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
			    Permission.RemoveTranslator,
			    Permission.SearchTranslatorForMember,
			    Permission.FullSearchTranslator,
			    Permission.SortTranslatorForMember,
			    Permission.FullSortTranslator,
			    Permission.ViewTranslatorDetails,
			    Permission.ViewAllTranslators))
		{
			return;
		}

		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			ConsoleHelper.ClearConsole();
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
					EditTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveTranslator,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.SearchTranslatorForMember, Permission.FullSearchTranslator))
						break;
					Console.Clear();
					SearchTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.SortTranslatorForMember, Permission.FullSortTranslator))
						break;
					Console.Clear();
					SortTranslators(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.ViewTranslatorDetails, Permission.ViewTranslatorBooks,
						    Permission.ViewAllTranslators))
						break;
					Console.Clear();
					ViewTranslators(translatorManagementService, authorization, session, statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
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
			(4, "Search Translator",
				authorization.HasAnyPermission(Permission.SearchTranslatorForMember, Permission.FullSearchTranslator)),
			(5, "Sort Translators",
				authorization.HasAnyPermission(Permission.SortTranslatorForMember, Permission.FullSortTranslator)),
			(6, "View Translators",
				authorization.HasAnyPermission(Permission.ViewTranslatorDetails, Permission.ViewTranslatorBooks,
					Permission.ViewAllTranslators)),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 33) + " TRANSLATOR MENU " + new string('=', 33));
		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems)
			Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
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


	private static readonly string[] Item = ["—"];


	private static void EditTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		Console.WriteLine(new string('=', 36) + " EDITING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			translator => MenuHelper.SelectTranslator(translator, authorization), Messages.NotAvailableTranslator);
		if (desiredTranslator is null) return;

		while (true)
		{
			Console.Clear();
			var headers = new[] { "#", "Field", "Current Value" };

			var rows = new List<string[][]>
			{
				new[] { ["1"], ["First Name"], new[] { desiredTranslator.FirstName } },
				new[] { ["2"], ["Last Name"], new[] { desiredTranslator.LastName } },
				new[] { ["3"], ["National Code"], new[] { desiredTranslator.NationalCode } },
				new[] { ["4"], ["Email"], new[] { desiredTranslator.Email } },
				new[] { ["5"], ["Phone Number"], new[] { desiredTranslator.PhoneNumber } },
				new[] { ["6"], ["Birth Date"], new[] { desiredTranslator.BirthDate.ToString("yyyy-MM-dd") } },
				new[] { ["7"], ["Back"], Item }
			};

			ConsoleTable.PrintTable("Edit Translator", headers, rows);

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 7);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var translatorNewFirstName = ConsoleHelper.GetValidName("\nEnter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewFirstName, v => new UpdateTranslatorDto { FirstName = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 2:
				{
					var translatorNewLastName = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewLastName, v => new UpdateTranslatorDto { LastName = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 3:
				{
					var translatorNewNationalCode = ConsoleHelper.GetValidNationalCode("\nEnter new national code");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewNationalCode, v => new UpdateTranslatorDto { NationalCode = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 4:
				{
					var translatorNewEmail = ConsoleHelper.GetValidEmail("\nEnter new email");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id, translatorNewEmail,
						v => new UpdateTranslatorDto { Email = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 5:
				{
					var translatorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");
					var updated = PerformUpdate(translatorManagementService, desiredTranslator.Id,
						translatorNewPhoneNumber, v => new UpdateTranslatorDto { PhoneNumber = v });
					if (updated is not null) desiredTranslator = updated;
					break;
				}
				case 6:
				{
					var translatorNewBirthDate = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");
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


	private static TranslatorDto? PerformUpdate<T>(TranslatorManagementService translatorManagementService,
		int desiredTranslatorId, T? newValue, Func<T, UpdateTranslatorDto> buildDto)
	{
		if (newValue is null) return null;

		var dto = buildDto(newValue);
		var result = translatorManagementService.UpdateTranslator(desiredTranslatorId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Data;
	}


	private static void RemoveTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		// TODO	(SQL Server)	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine(new string('=', 36) + " REMOVING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			translator => MenuHelper.SelectTranslator(translator, authorization), Messages.NotAvailableTranslator);

		PersonHelper.PerformRemove(desiredTranslator, desiredTranslator?.FirstName ?? "",
			desiredTranslator?.LastName ?? "", TranslatorPrinter.PrintDetails,
			() => translatorManagementService.RemoveTranslator(desiredTranslator!.Id));
	}


	private static void SearchTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasAnyPermission(Permission.SearchTranslatorForMember, Permission.FullSearchTranslator))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Action<IReadOnlyList<TranslatorDto>> printer =
			authorization.HasAnyPermission(Permission.ViewTranslatorDetails, Permission.FullSearchTranslator)
				? translator => TranslatorPrinter.PrintFullTable(translator, "Search Result")
				: translator => TranslatorPrinter.PrintTable(translator, "Search Result");

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING TRANSLATOR MENU " + new string('=', 36));

			if (translatorManagementService.GetAllTranslators().Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
				return;
			}

			var items = new List<(TranslatorSearchField? Field, string Label, Permission[] RequiredPermissions)>
			{
				(TranslatorSearchField.Name, "Name",
					[Permission.FullSearchTranslator, Permission.SearchTranslatorForMember]),
				(TranslatorSearchField.NationalCode, "National Code", [Permission.FullSearchTranslator]),
				(TranslatorSearchField.Email, "Email",
					[Permission.FullSearchTranslator, Permission.SearchTranslatorForMember]),
				(TranslatorSearchField.PhoneNumber, "Phone Number", [Permission.FullSearchTranslator]),
			};

			var available = items.Where(i =>
				i.RequiredPermissions.Length == 0 || authorization.HasAnyPermission(i.RequiredPermissions)).ToList();

			var displayNumber = 1;
			foreach (var (_, label, _) in available)
				Console.WriteLine($"{displayNumber++}. {label}");
			Console.WriteLine($"{displayNumber}. Back");

			var choice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, displayNumber);
			if (choice is null) return;

			if (choice == displayNumber) // Back
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Translator"));
				return;
			}

			var (field, _, _) = available[choice.Value - 1];

			var prompt = field switch
			{
				TranslatorSearchField.Name => Messages.SearchName,
				TranslatorSearchField.NationalCode => Messages.SearchNationalCode,
				TranslatorSearchField.Email => Messages.SearchEmail,
				TranslatorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			PersonHelper.SearchAndDisplay(prompt,
				term => translatorManagementService.SearchTranslator(term, field!.Value), printer,
				Messages.NotTranslatorMatched);

			ConsoleHelper.Pause();
		}
	}


	private static void SortTranslators(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasAnyPermission(Permission.SortTranslatorForMember, Permission.FullSortTranslator))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		var sortFields = GetAvailableTranslatorSortFields(authorization);
		while (true)
		{
			Console.Clear();
			var fieldHeaders = new[] { "#", "Sort By" };
			var fieldRows = sortFields
				.Select((field, index) => new string[][] { [(index + 1).ToString()], [field.Label] }).ToList();
			fieldRows.Add([[(sortFields.Count + 1).ToString()], ["Back"]]);


			ConsoleTable.PrintTable("Sort Translator", fieldHeaders, fieldRows);

			var backOption = sortFields.Count + 1;
			var choice = ConsoleHelper.ReadInt(Messages.SortFieldQuestion, 1, backOption);
			if (choice is null) return;

			if (choice == backOption)
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SortCancelled, "Translator"));
				return;
			}

			var (selectedField, _) = sortFields[choice.Value - 1];
			var sortDirection = SelectSortDirection();
			if (sortDirection is null) continue;

			var sortedTranslators =
				translatorManagementService.GetAllTranslators(selectedField, sortDirection.Value);
			if (sortedTranslators.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
				continue;
			}

			var sortDescription = $"{selectedField} ({sortDirection})";

			var canViewFullDetails = authorization.HasPermission(Permission.FullSortTranslator);
			if (canViewFullDetails)
				TranslatorPrinter.PrintFullTable(sortedTranslators, $"Sorted Translators - {sortDescription}");
			else
				TranslatorPrinter.PrintTable(sortedTranslators, $"Sorted Translators - {sortDescription}");

			ConsoleHelper.Pause();
		}
	}


	private static List<(TranslatorSortField Field, string Label)> GetAvailableTranslatorSortFields(
		IAuthorizationService authorization)
	{
		var fields = new List<(TranslatorSortField Field, string Label)>
		{
			(TranslatorSortField.Id, "ID"),
			(TranslatorSortField.FirstName, "First Name"),
			(TranslatorSortField.LastName, "Last Name"),
			(TranslatorSortField.FullName, "Full Name"),
			(TranslatorSortField.Email, "Email")
		};

		if (!authorization.HasPermission(Permission.FullSortAuthor)) return fields;
		fields.Insert(3, (TranslatorSortField.NationalCode, "National Code"));
		fields.Add((TranslatorSortField.BirthDate, "Birth Date"));
		fields.Add((TranslatorSortField.BookCount, "Book Count"));
		return fields;
	}


	private static SortDirection? SelectSortDirection()
	{
		var directionHeaders = new[] { "#", "Direction" };
		var directionRows = new List<string[][]>
		{
			new string[][] { ["1"], ["Ascending"] },
			new string[][] { ["2"], ["Descending"] },
			new string[][] { ["3"], ["Back"] }
		};
		ConsoleTable.PrintTable("Sort Direction", directionHeaders, directionRows);

		var directionChoice = ConsoleHelper.ReadInt(Messages.SortDirectionQuestion, 1, 3);
		if (directionChoice is null) return null;

		return directionChoice.Value switch
		{
			1 => SortDirection.Ascending,
			2 => SortDirection.Descending,
			_ => null
		};
	}


	private static void ViewTranslators(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization, ICurrentUserSession session, LibraryStatisticsService statisticsService)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
			    Permission.ViewTranslatorDetails,
			    Permission.ViewTranslatorBooks,
			    Permission.ViewAllTranslators))
		{
			return;
		}

		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (ViewTranslatorMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewTranslatorDetails,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
						translator => MenuHelper.SelectTranslator(translator, authorization),
						Messages.NotAvailableTranslator);
					if (desiredTranslator is not null) TranslatorPrinter.PrintDetails(desiredTranslator);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewBooksByTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllTranslators,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var translators = translatorManagementService.GetAllTranslators();
					if (translators.Count == 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					else if (authorization.HasPermission(Permission.ViewTranslatorFullDetails))
						TranslatorPrinter.PrintFullTable(translators);
					else
						TranslatorPrinter.PrintTable(translators);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewRemovedTranslators,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var removedTranslators = translatorManagementService.GetRemovedTranslators();
					if (removedTranslators.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableRemovedTranslator);
					else
						TranslatorPrinter.PrintFullTable(removedTranslators);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo(Messages.BackToTranslatorMenu);
					return;
				}
			}
		}
	}


	private static int ViewTranslatorMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "View Translator Details", authorization.HasPermission(Permission.ViewTranslatorDetails)),
			(2, "View Translator's Books", authorization.HasPermission(Permission.ViewTranslatorBooks)),
			(3, "View All Translators", authorization.HasPermission(Permission.ViewAllTranslators)),
			(4, "View Removed Translators", authorization.HasPermission(Permission.ViewRemovedTranslators)),
			(5, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 33) + " VIEW TRANSLATOR MENU " + new string('=', 33));
		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems)
			Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
	}


	private static void ViewBooksByTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!SessionGuard.RequirePermission(authorization, Permission.ViewTranslatorBooks, Messages.AccessDenied))
			return;
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			translator => MenuHelper.SelectTranslator(translator, authorization), Messages.NotAvailableTranslator);
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