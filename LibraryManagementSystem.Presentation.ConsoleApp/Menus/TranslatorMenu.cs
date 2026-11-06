using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public class TranslatorMenu
{
	public static void TranslatorMenuController(TranslatorManagementService translatorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization,
		IMenuService menuService)
	{
		if (!authorization.HasAnyPermission(
			    Permission.AddContributor,
			    Permission.EditContributor,
			    Permission.RemoveContributor,
			    Permission.SearchForMember,
			    Permission.FullSearchContributor,
			    Permission.FullSortContributor,
			    Permission.ViewContributorDetails,
			    Permission.ViewAllContributors))
			return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (TranslatorMenuList(menuService))
			{
				case "translators.add":
				{
					if (!authorization.HasPermission(Permission.AddContributor)) break;
					Console.Clear();
					AddTranslator(translatorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.edit":
				{
					if (!authorization.HasPermission(Permission.EditContributor)) break;
					Console.Clear();
					EditTranslator(translatorManagementService, authorization, session, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators.remove":
				{
					if (!authorization.HasPermission(Permission.RemoveContributor)) break;
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
		var translatorDto = ContributorMenu.PromptForContributorDto("translator");
		if (translatorDto is null) return;

		var result = translatorManagementService.AddTranslator(translatorDto);
		ConsoleHelper.ShowResult(result);
	}


	private static void EditTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization, ICurrentUserSession session, IMenuService menuService)
	{
		Console.WriteLine(new string('=', 36) + " EDITING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			translator => MenuHelper.SelectTranslator(translator, authorization), Messages.NotAvailableTranslator);
		if (desiredTranslator is null) return;

		ContributorMenu.EditContributor(desiredTranslator, "Translator", "translators", menuService,
			translatorManagementService.UpdateTranslator, session.CurrentUser.Id);
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
				"translators.search.name" => ContributorSearchField.Name,
				"translators.search.national-code" => ContributorSearchField.NationalCode,
				"translators.search.email" => ContributorSearchField.Email,
				"translators.search.phone-number" => ContributorSearchField.PhoneNumber,
				_ => (ContributorSearchField?)null
			};

			if (field is null) continue;

			var prompt = field switch
			{
				ContributorSearchField.Name => Messages.SearchName,
				ContributorSearchField.NationalCode => Messages.SearchNationalCode,
				ContributorSearchField.Email => Messages.SearchEmail,
				ContributorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
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
		if (!authorization.HasAnyPermission(Permission.ViewContributorDetails, Permission.ViewContributorBooks,
			    Permission.ViewAllContributors))
			return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (ViewTranslatorMenuList(menuService))
			{
				case "translators.view.translator-details":
				{
					if (!authorization.HasPermission(Permission.ViewContributorDetails)) break;
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
					if (!authorization.HasPermission(Permission.ViewAllContributors)) break;
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
					if (!authorization.HasPermission(Permission.ViewRemovedContributors)) break;
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
		if (!authorization.HasPermission(Permission.ViewContributorBooks)) return;
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