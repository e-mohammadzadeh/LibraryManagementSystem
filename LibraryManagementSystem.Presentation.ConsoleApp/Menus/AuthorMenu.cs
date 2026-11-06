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

public class AuthorMenu
{
	public static void AuthorMenuController(AuthorManagementService authorManagementService,
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
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (AuthorMenuList(menuService))
			{
				case "authors.add":
				{
					if (!authorization.HasPermission(Permission.AddContributor)) break;

					Console.Clear();
					AddAuthor(authorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.edit":
				{
					if (!authorization.HasPermission(Permission.EditContributor)) break;
					Console.Clear();
					EditAuthor(authorManagementService, authorization, session, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.remove":
				{
					if (!authorization.HasPermission(Permission.RemoveContributor)) break;
					Console.Clear();
					RemoveAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.search":
				{
					if (!authorization.HasAnyPermission(Permission.SearchForMember, Permission.FullSearchContributor))
						break;
					Console.Clear();
					SearchAuthor(authorManagementService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.sort":
				{
					Console.Clear();
					SortAuthors(authorManagementService, menuService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.view":
				{
					if (!authorization.HasAnyPermission(Permission.ViewContributorDetails,
						    Permission.ViewContributorBooks, Permission.ViewAllContributors))
						break;
					Console.Clear();
					ViewAuthors(authorManagementService, authorization, session, statisticsService, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? AuthorMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("authors");
		return MenuHelper.DisplayMenu("AUTHOR MENU", items);
	}



	private static void AddAuthor(AuthorManagementService authorManagementService)
	{
		Console.WriteLine(new string('=', 36) + " ADDING AUTHOR MENU " + new string('=', 36));
		var authorDto = ContributorMenu.PromptForContributorDto("author");
		if (authorDto is null) return;

		var result = authorManagementService.AddAuthor(authorDto);
		ConsoleHelper.ShowResult(result);
	}


	private static void EditAuthor(AuthorManagementService authorManagementService, IAuthorizationService authorization,
		ICurrentUserSession session, IMenuService menuService)
	{
		Console.WriteLine(new string('=', 36) + " EDITING AUTHOR MENU " + new string('=', 36));

		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
			author => MenuHelper.SelectAuthor(author, authorization),
			Messages.NotAvailableAuthor);
		if (desiredAuthor is null) return;

		ContributorMenu.EditContributor(desiredAuthor, "Author", "authors", menuService,
			authorManagementService.UpdateAuthor, session.CurrentUser.Id);
	}


	private static void RemoveAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		Console.WriteLine(new string('=', 36) + " REMOVING AUTHOR MENU " + new string('=', 36));
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
			author => MenuHelper.SelectAuthor(author, authorization),
			Messages.NotAvailableAuthor);

		ContributorMenu.RemoveContributor(desiredAuthor, desiredAuthor?.FirstName ?? "", desiredAuthor?.LastName ?? "",
			AuthorPrinter.PrintDetails, () => authorManagementService.RemoveAuthor(desiredAuthor!.Id));
	}



	private static void SearchAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization, IMenuService menuService)
	{
		ContributorMenu.SearchContributor(authorManagementService.GetAllAuthors(), "Author", "authors", menuService,
			authorization, authorManagementService.SearchAuthor, Messages.NotAuthorMatched);
	}


	private static void SortAuthors(AuthorManagementService authorManagementService, IMenuService menuService,
		IAuthorizationService authorization)
	{
		MenuHelper.SortByMenu<ContributorSortField, ContributorDto>(
			menuService,
			sortMenuId: "authors.sort",
			menuTitle: "SORT AUTHORS",
			mapField: TryMapAuthorSortField,
			loadSorted: authorManagementService.GetAllAuthors,
			display: (authors, description) =>
			{
				var title = $"Sorted Authors - {description}";
				if (authorization.HasPermission(Permission.FullSortContributor))
					AuthorPrinter.PrintFullTable(authors, title);
				else
					AuthorPrinter.PrintTable(authors, title);
			},
			emptyMessage: Messages.NotAvailableAuthor,
			entityNameForCancel: "Author",
			backMenuId: "authors.sort.back");
	}


	private static bool TryMapAuthorSortField(string menuId, out ContributorSortField field)
	{
		switch (menuId.ToLowerInvariant())
		{
			case "authors.sort.firstname":
				field = ContributorSortField.FirstName;
				return true;
			case "authors.sort.lastname":
				field = ContributorSortField.LastName;
				return true;
			case "authors.sort.fullname":
				field = ContributorSortField.FullName;
				return true;
			case "authors.sort.email":
				field = ContributorSortField.Email;
				return true;
			case "authors.sort.national-code":
				field = ContributorSortField.NationalCode;
				return true;
			case "authors.sort.birthdate":
				field = ContributorSortField.BirthDate;
				return true;
			case "authors.sort.book-count":
				field = ContributorSortField.BookCount;
				return true;
			default:
				field = default;
				return false;
		}
	}


	private static void ViewAuthors(AuthorManagementService authorManagementService,
		IAuthorizationService authorization, ICurrentUserSession session, LibraryStatisticsService statisticsService,
		IMenuService menuService)
	{
		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (ViewAuthorMenuList(menuService))
			{
				case "authors.view.author-details":
				{
					if (!authorization.HasPermission(Permission.ViewContributorDetails)) break;
					Console.Clear();
					var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
						author => MenuHelper.SelectAuthor(author, authorization), Messages.NotAvailableAuthor);
					if (desiredAuthor is not null) AuthorPrinter.PrintDetails(desiredAuthor);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.view.author-books":
				{
					Console.Clear();
					ViewBooksByAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.view.all-authors":
				{
					if (!authorization.HasPermission(Permission.ViewAllContributors)) break;
					Console.Clear();
					var authors = authorManagementService.GetAllAuthors();

					if (authors.Count == 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableAuthor);
					else if (authorization.HasPermission(Permission.ViewContributorDetails))
						AuthorPrinter.PrintFullTable(authors);
					else
						AuthorPrinter.PrintTable(authors);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.view.removed-authors":
				{
					if (!authorization.HasPermission(Permission.ViewRemovedContributors)) break;
					Console.Clear();
					var removedAuthors = authorManagementService.GetRemovedAuthors();
					if (removedAuthors.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableRemovedAuthor);
					else
						AuthorPrinter.PrintFullTable(removedAuthors);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.view.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToAuthorMenu);
					return;
				}
			}
		}
	}


	private static string? ViewAuthorMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("authors.view");
		return MenuHelper.DisplayMenu("VIEW AUTHOR MENU", items);
	}


	private static void ViewBooksByAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.ViewContributorBooks)) return;
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
			author => MenuHelper.SelectAuthor(author, authorization), Messages.NotAvailableAuthor);
		if (desiredAuthor is null) return;

		var books = authorManagementService.GetBooksByAuthor(desiredAuthor.Id);
		if (books.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.AuthorHasNoBooks);
			return;
		}

		BookPrinter.PrintTable(books, authorization);
	}
}