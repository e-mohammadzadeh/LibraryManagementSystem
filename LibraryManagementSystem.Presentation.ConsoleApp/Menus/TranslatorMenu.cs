using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
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
		Console.WriteLine(new string('=', 36) + " REMOVING TRANSLATOR MENU " + new string('=', 36));
		var desiredTranslator = MenuHelper.SelectExisting(translatorManagementService.GetAllTranslators(),
			translator => MenuHelper.SelectTranslator(translator, authorization), Messages.NotAvailableTranslator);

		ContributorMenu.RemoveContributor(desiredTranslator, desiredTranslator?.FirstName ?? "",
			desiredTranslator?.LastName ?? "", x => ContributorPrinter.PrintDetails(x, "Remove"),
			() => translatorManagementService.RemoveTranslator(desiredTranslator!.Id));
	}


	private static void SearchTranslator(TranslatorManagementService translatorManagementService,
		IAuthorizationService authorization, IMenuService menuService)
	{
		ContributorMenu.SearchContributor(translatorManagementService.GetAllTranslators(), "Translator", "translators",
			menuService, authorization, translatorManagementService.SearchTranslator, Messages.NotTranslatorMatched);
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
					ContributorPrinter.PrintFullTable(list, title);
				else
					ContributorPrinter.PrintTable(list, title);
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


	private static void ViewTranslators(
		TranslatorManagementService translatorManagementService,
		IMenuService menuService,
		IAuthorizationService authorization,
		ICurrentUserSession session,
		LibraryStatisticsService statisticsService)
	{
		ContributorMenu.ViewContributors(
			contributorName: "Translator",
			menuIdPrefix: "translators",
			backMessage: Messages.BackToTranslatorMenu,
			unavailableMessage: Messages.NotAvailableTranslator,
			unavailableRemovedMessage: Messages.NotAvailableRemovedTranslator,
			menuService: menuService,
			authorization: authorization,
			session: session,
			statisticsService: statisticsService,
			getAllContributors: () => translatorManagementService.GetAllTranslators(),
			getRemovedContributors: translatorManagementService.GetRemovedTranslators,
			selectContributor: () => MenuHelper.SelectExisting(
				translatorManagementService.GetAllTranslators(),
				translator => MenuHelper.SelectTranslator(translator, authorization),
				Messages.NotAvailableTranslator),
			viewBooks: () => ViewBooksByTranslator(translatorManagementService, authorization),
			requireViewPermission: true);
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