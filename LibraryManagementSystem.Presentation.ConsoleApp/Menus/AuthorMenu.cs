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

public static class AuthorMenu
{
	public static void AuthorMenuController(AuthorManagementService authorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization,
		IMenuService menuService)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
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
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (AuthorMenuList(menuService))
			{
				case "authors.add":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					AddAuthor(authorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.edit":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					EditAuthor(authorManagementService, authorization, session);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.remove":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveContributor,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "authors.search":
				{
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


	public static ContributorDto? PromptForAuthorDto()
	{
		var fields = PersonHelper.PromptForPersonFields("author");
		if (fields is null) return null;

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional)", true);

		var email = Email.Create(fields.Email);
		var phoneNumber = PhoneNumber.Create(fields.PhoneNumber);

		return new ContributorDto
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = email, PhoneNumber = phoneNumber, BirthDate = fields.BirthDate, Biography = biography
		};
	}


	private static void AddAuthor(AuthorManagementService authorManagementService)
	{
		Console.WriteLine(new string('=', 36) + " ADDING AUTHOR MENU " + new string('=', 36));
		var authorDto = PromptForAuthorDto();
		if (authorDto is null) return;

		var result = authorManagementService.AddAuthor(authorDto);
		ConsoleHelper.ShowResult(result);
	}


	private static readonly string[] Item = ["—"];


	private static void EditAuthor(AuthorManagementService authorManagementService, IAuthorizationService authorization,
		ICurrentUserSession session)
	{
		Console.WriteLine(new string('=', 36) + " EDITING AUTHOR MENU " + new string('=', 36));
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
			author => MenuHelper.SelectAuthor(author, authorization),
			Messages.NotAvailableAuthor);
		if (desiredAuthor is null) return;

		while (true)
		{
			Console.Clear();
			var headers = new[] { "#", "Field", "Current Value" };

			var rows = new List<string[][]>
			{
				new[] { ["1"], ["First Name"], new[] { desiredAuthor.FirstName } },
				new[] { ["2"], ["Last Name"], new[] { desiredAuthor.LastName } },
				new[] { ["3"], ["National Code"], new[] { desiredAuthor.NationalCode } },
				new[] { ["4"], ["Email"], new[] { desiredAuthor.Email.ToString() } },
				new[] { ["5"], ["Phone Number"], new[] { desiredAuthor.PhoneNumber.ToString() } },
				new[] { ["6"], ["Birth Date"], new[] { desiredAuthor.BirthDate.ToString("yyyy-MM-dd") } },
				new[]
				{
					["7"], ["Biography"],
					string.IsNullOrWhiteSpace(desiredAuthor.Biography)
						? ["—"]
						: new[] { desiredAuthor.Biography }
				},
				new[] { ["8"], ["Back"], Item }
			};

			ConsoleTable.PrintTable("Edit Author", headers, rows);

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 8);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var authorNewFirstName = ConsoleHelper.GetValidName("\nEnter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);
					if (authorNewFirstName != null)
					{
						desiredAuthor.FirstName = authorNewFirstName;
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					if (authorNewLastName != null)
					{
						desiredAuthor.LastName = authorNewLastName;
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("\nEnter new national code");

					if (authorNewNationalCode != null)
					{
						desiredAuthor.NationalCode = authorNewNationalCode;
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("\nEnter new email");

					if (authorNewEmail != null)
					{
						desiredAuthor.Email = Email.Create(authorNewEmail);
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 5:
				{
					Console.Clear();
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");

					if (authorNewPhoneNumber != null)
					{
						desiredAuthor.PhoneNumber = PhoneNumber.Create(authorNewPhoneNumber);
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");

					if (authorNewBirthDate != null)
					{
						desiredAuthor.BirthDate = authorNewBirthDate.Value;
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("\nEnter new biography");

					if (authorNewBiography != null)
					{
						desiredAuthor.Biography = authorNewBiography;
						var result = authorManagementService.UpdateAuthor(desiredAuthor, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "Author"));
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
		}
	}


	private static void RemoveAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		// TODO	(SQL Server)	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine(new string('=', 36) + " REMOVING AUTHOR MENU " + new string('=', 36));
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
			author => MenuHelper.SelectAuthor(author, authorization),
			Messages.NotAvailableAuthor);

		PersonHelper.PerformRemove(desiredAuthor, desiredAuthor?.FirstName ?? "", desiredAuthor?.LastName ?? "",
			AuthorPrinter.PrintDetails, () => authorManagementService.RemoveAuthor(desiredAuthor!.Id));
	}



	private static void SearchAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization, IMenuService menuService)
	{
		if (!authorization.HasAnyPermission(Permission.SearchForMember, Permission.FullSearchContributor))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		if (authorManagementService.GetAllAuthors().Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableAuthor);
			return;
		}

		Action<IReadOnlyList<ContributorDto>> printer = authorization.HasAnyPermission(Permission.FullSearchContributor)
			? author => AuthorPrinter.PrintFullTable(author, "Search Result")
			: author => AuthorPrinter.PrintTable(author, "Search Result");

		while (true)
		{
			Console.Clear();
			var items = menuService.GetChildren("authors.search");
			var selectedItem = MenuHelper.DisplayMenu("SEARCHING AUTHOR MENU", items);
			switch (selectedItem)
			{
				case null:
					return;
				case "authors.search.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Author"));
					return;
			}

			var field = selectedItem switch
			{
				"authors.search.name" => AuthorSearchField.Name,
				"authors.search.national-code" => AuthorSearchField.NationalCode,
				"authors.search.email" => AuthorSearchField.Email,
				"authors.search.phone-number" => AuthorSearchField.PhoneNumber,
				_ => (AuthorSearchField?)null
			};

			if (field is null) continue;

			var prompt = field.Value switch
			{
				AuthorSearchField.Name => Messages.SearchName,
				AuthorSearchField.NationalCode => Messages.SearchNationalCode,
				AuthorSearchField.Email => Messages.SearchEmail,
				AuthorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			PersonHelper.SearchAndDisplay(prompt, term => authorManagementService.SearchAuthor(term, field.Value),
				printer, Messages.NotAuthorMatched);
			ConsoleHelper.Pause();
		}
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
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied, Permission.ViewContributorDetails,
			    Permission.ViewContributorBooks, Permission.ViewAllContributors))
			return;

		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (ViewAuthorMenuList(menuService))
			{
				case "authors.view.author-details":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewContributorDetails,
						    Messages.AccessDenied))
						break;
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
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllContributors,
						    Messages.AccessDenied))
						break;

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
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewRemovedContributors,
						    Messages.AccessDenied))
						break;
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
		if (!SessionGuard.RequirePermission(authorization, Permission.ViewContributorBooks, Messages.AccessDenied))
			return;
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