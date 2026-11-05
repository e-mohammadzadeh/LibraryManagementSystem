using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class TranslatorMenu
{
	public static void TranslatorMenuController(TranslatorManagementService translatorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization,
		IMenuService menuService)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.AddContributor,
			    Permission.EditContributor,
			    Permission.RemoveContributor,
			    Permission.SearchForMember,
			    Permission.FullSearchContributor,
			    Permission.FullSortContributor,
			    Permission.ViewContributorDetails,
			    Permission.ViewAllContributors))
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
			switch (TranslatorMenuList(menuService))
			{
				case "translators.add":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					AddTranslator(translatorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.edit":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					EditTranslator(translatorManagementService, authorization, session, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.remove":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.search":
				{
					Console.Clear();
					SearchTranslator(translatorManagementService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.sort":
				{
					Console.Clear();
					SortTranslators(translatorManagementService, menuService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.view":
				{
					Console.Clear();
					ViewTranslators(translatorManagementService, menuService, authorization, session,
						statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? TranslatorMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("translators");
		return MenuHelper.DisplayMenu("TRANSLATOR MENU", items);
	}


	private static void AddTranslator(TranslatorManagementService translatorManagementService)
	{
		Console.WriteLine(new string('=', 36) + " ADDING TRANSLATOR MENU " + new string('=', 36));
		var translatorDto = PromptForTranslatorDto();
		if (translatorDto is null) return;

		var result = translatorManagementService.AddTranslator(translatorDto);
		ConsoleHelper.ShowResult(result);
	}


	public static ContributorDto? PromptForTranslatorDto()
	{
		var fields = PersonHelper.PromptForPersonFields("translator");
		if (fields is null) return null;

		var email = Email.Create(fields.Email);
		var phoneNumber = PhoneNumber.Create(fields.PhoneNumber);
		return new ContributorDto
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = email, PhoneNumber = phoneNumber, BirthDate = fields.BirthDate
		};
	}



	private static void EditTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization, ICurrentUserSession session, IMenuService menuService)
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
				new[] { ["First Name"], new[] { desiredTranslator.FirstName } },
				new[] { ["Last Name"], new[] { desiredTranslator.LastName } },
				new[] { ["National Code"], new[] { desiredTranslator.NationalCode } },
				new[] { ["Email"], new[] { desiredTranslator.Email.ToString() } },
				new[] { ["Phone Number"], new[] { desiredTranslator.PhoneNumber.ToString() } },
				new[] { ["Birth Date"], new[] { desiredTranslator.BirthDate.ToString("yyyy-MM-dd") } },
				new[]
				{
					["Biography"],
					new[] { string.IsNullOrWhiteSpace(desiredTranslator.Biography) ? "-" : desiredTranslator.Biography }
				},
			};

			ConsoleTable.PrintTable("TRANSLATOR DETAILS", headers, rows);

			var items = menuService.GetChildren("translators.edit");

			var selectedItem = MenuHelper.DisplayMenu("EDIT TRANSLATOR MENU", items);

			switch (selectedItem)
			{
				case null:
					return;
				case "translators.edit.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "Translator"));
					return;
			}

			var updated = true;

			switch (selectedItem)
			{
				case "translators.edit.firstname":
				{
					var value = ConsoleHelper.GetValidName("\nEnter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					if (value != null)
						desiredTranslator.FirstName = value;
					else
						updated = false;
					break;
				}
				case "translators.edit.lastname":
				{
					var value = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					if (value != null)
						desiredTranslator.LastName = value;
					else
						updated = false;
					break;
				}
				case "translators.edit.national-code":
				{
					var value = ConsoleHelper.GetValidNationalCode("\nEnter new national code");

					if (value != null)
						desiredTranslator.NationalCode = value;
					else
						updated = false;
					break;
				}
				case "translators.edit.email":
				{
					var value = ConsoleHelper.GetValidEmail("\nEnter new email");

					if (value != null)
						desiredTranslator.Email = Email.Create(value);
					else
						updated = false;
					break;
				}
				case "translators.edit.phone-number":
				{
					var value = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");

					if (value != null)
						desiredTranslator.PhoneNumber = PhoneNumber.Create(value);
					else
						updated = false;
					break;
				}
				case "translators.edit.birthdate":
				{
					var value = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");

					if (value != null)
						desiredTranslator.BirthDate = value.Value;
					else
						updated = false;
					break;
				}
				case "translators.edit.biography":
				{
					var value = ConsoleHelper.ReadString("\nEnter new biography");

					if (value != null)
						desiredTranslator.Biography = value;
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
				var result = translatorManagementService.UpdateTranslator(desiredTranslator, session.CurrentUser.Id);
				ConsoleHelper.ShowResult(result);
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
		}
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
		IAuthorizationService authorization, IMenuService menuService)
	{
		if (!authorization.HasAnyPermission(Permission.SearchForMember, Permission.FullSearchContributor))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		if (translatorManagementService.GetAllTranslators().Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
			return;
		}

		Action<IReadOnlyList<ContributorDto>> printer =
			authorization.HasAnyPermission(Permission.ViewContributorDetails, Permission.FullSearchContributor)
				? translator => TranslatorPrinter.PrintFullTable(translator, "Search Result")
				: translator => TranslatorPrinter.PrintTable(translator, "Search Result");

		while (true)
		{
			Console.Clear();
			var items = menuService.GetChildren("translators.search");
			var selectedItem = MenuHelper.DisplayMenu("SEARCHING TRANSLATOR MENU", items);
			switch (selectedItem)
			{
				case null:
					return;
				case "translators.search.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Translator"));
					return;
			}

			var field = selectedItem switch
			{
				"translators.search.name" => TranslatorSearchField.Name,
				"translators.search.national-code" => TranslatorSearchField.NationalCode,
				"translators.search.email" => TranslatorSearchField.Email,
				"translators.search.phone-number" => TranslatorSearchField.PhoneNumber,
				_ => (TranslatorSearchField?)null
			};

			if (field is null) continue;

			var prompt = field switch
			{
				TranslatorSearchField.Name => Messages.SearchName,
				TranslatorSearchField.NationalCode => Messages.SearchNationalCode,
				TranslatorSearchField.Email => Messages.SearchEmail,
				TranslatorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			PersonHelper.SearchAndDisplay(prompt,
				term => translatorManagementService.SearchTranslator(term, field.Value), printer,
				Messages.NotTranslatorMatched);

			ConsoleHelper.Pause();
		}
	}


	private static void SortTranslators(TranslatorManagementService translatorManagementService,
		IMenuService menuService,
		IAuthorizationService authorization)
	{
		MenuHelper.SortByMenu<ContributorSortField, ContributorDto>(
			menuService,
			"translators.sort",
			"SORT TRANSLATORS",
			TryMapTranslatorSortField,
			translatorManagementService.GetAllTranslators,
			(list, description) =>
			{
				var title = $"Sorted Translators - {description}";
				if (authorization.HasPermission(Permission.FullSortContributor))
					TranslatorPrinter.PrintFullTable(list, title);
				else
					TranslatorPrinter.PrintTable(list, title);
			},
			Messages.NotAvailableTranslator,
			"Translator",
			"translators.sort.back");
	}


	private static bool TryMapTranslatorSortField(string menuId, out ContributorSortField field)
	{
		switch (menuId.ToLowerInvariant())
		{
			case "translators.sort.firstname":
				field = ContributorSortField.FirstName;
				return true;
			case "translators.sort.lastname":
				field = ContributorSortField.LastName;
				return true;
			case "translators.sort.fullname":
				field = ContributorSortField.FullName;
				return true;
			case "translators.sort.email":
				field = ContributorSortField.Email;
				return true;
			case "translators.sort.national-code":
				field = ContributorSortField.NationalCode;
				return true;
			case "translators.sort.birthdate":
				field = ContributorSortField.BirthDate;
				return true;
			case "translators.sort.book-count":
				field = ContributorSortField.BookCount;
				return true;
			default:
				field = default;
				return false;
		}
	}


	private static void ViewTranslators(TranslatorManagementService translatorManagementService,
		IMenuService menuService,
		IAuthorizationService authorization, ICurrentUserSession session, LibraryStatisticsService statisticsService)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
			    Permission.ViewContributorDetails, Permission.ViewContributorBooks, Permission.ViewAllContributors))
			return;

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
			switch (ViewTranslatorMenuList(menuService))
			{
				case "translators.view.translator-details":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewContributorDetails,
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
				case "translators.view.translator-books":
				{
					Console.Clear();
					ViewBooksByTranslator(translatorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.view.all-translators":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllContributors,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var translators = translatorManagementService.GetAllTranslators();
					if (translators.Count == 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					else if (authorization.HasPermission(Permission.ViewContributorDetails))
						TranslatorPrinter.PrintFullTable(translators);
					else
						TranslatorPrinter.PrintTable(translators);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.view.removed-translators":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewRemovedContributors,
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
				case "translators.view.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToTranslatorMenu);
					return;
				}
			}
		}
	}


	private static string? ViewTranslatorMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("translators.view");
		return MenuHelper.DisplayMenu("VIEW TRANSLATOR MENU", items);
	}


	private static void ViewBooksByTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization)
	{
		if (!SessionGuard.RequirePermission(authorization, Permission.ViewContributorBooks, Messages.AccessDenied))
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