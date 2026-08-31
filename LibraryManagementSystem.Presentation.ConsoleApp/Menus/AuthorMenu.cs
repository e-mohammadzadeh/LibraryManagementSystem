using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Search;
using LibraryManagementSystem.Domain.Enums.Sort;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class AuthorMenu
{
	public static void AuthorMenuController(AuthorManagementService authorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
			    Permission.AddAuthor,
			    Permission.EditAuthor,
			    Permission.RemoveAuthor,
			    Permission.SearchAuthorForMember,
			    Permission.FullSearchAuthor,
			    Permission.SortAuthorForMember,
			    Permission.FullSortAuthor,
			    Permission.ViewAuthorDetails,
			    Permission.ViewAllAuthors))
		{
			return;
		}

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
			switch (AuthorMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddAuthor, Messages.AccessDenied))
						break;
					Console.Clear();
					AddAuthor(authorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditAuthor, Messages.AccessDenied))
						break;
					Console.Clear();
					EditAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveAuthor, Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.SearchAuthorForMember, Permission.FullSearchAuthor))
						break;
					SearchAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.SortAuthorForMember, Permission.FullSortAuthor))
						break;
					Console.Clear();
					SortAuthors(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.ViewAuthorDetails, Permission.ViewAuthorBooks,
						    Permission.ViewAllAuthors))
						break;
					Console.Clear();
					ViewAuthorMenu(authorManagementService, authorization, session, statisticsService);
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


	private static int AuthorMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Add Author", authorization.HasPermission(Permission.AddAuthor)),
			(2, "Edit Author", authorization.HasPermission(Permission.EditAuthor)),
			(3, "Remove Author", authorization.HasPermission(Permission.RemoveAuthor)),
			(4, "Search Author",
				authorization.HasAnyPermission(Permission.SearchAuthorForMember, Permission.FullSearchAuthor)),
			(5, "Sort Authors",
				authorization.HasAnyPermission(Permission.SortAuthorForMember, Permission.FullSortAuthor)),
			(6, "View Authors",
				authorization.HasAnyPermission(Permission.ViewAuthorDetails, Permission.ViewAuthorBooks,
					Permission.ViewAllAuthors)),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 35) + " AUTHOR MENU " + new string('=', 35));

		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
	}


	public static CreateAuthorDto? PromptForAuthorDto()
	{
		var fields = PersonHelper.PromptForPersonFields("author");
		if (fields is null) return null;

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional)", true);

		return new CreateAuthorDto
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = fields.Email, PhoneNumber = fields.PhoneNumber, BirthDate = fields.BirthDate, Biography = biography
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


	private static void EditAuthor(AuthorManagementService authorManagementService, IAuthorizationService authorization)
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
				new[] { ["4"], ["Email"], new[] { desiredAuthor.Email } },
				new[] { ["5"], ["Phone Number"], new[] { desiredAuthor.PhoneNumber } },
				new[] { ["6"], ["Birth Date"], new[] { desiredAuthor.BirthDate.ToString("yyyy-MM-dd") } },
				new[]
				{
					["7"], ["Biography"],
					string.IsNullOrWhiteSpace(desiredAuthor.Biography)
						? ["—"]
						: ConsoleTable.WrapText(desiredAuthor.Biography, 40)
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
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewFirstName,
						v => new UpdateAuthorDto { FirstName = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewLastName,
						v => new UpdateAuthorDto { LastName = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("\nEnter new national code");
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewNationalCode,
						v => new UpdateAuthorDto { NationalCode = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("\nEnter new email");
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewEmail,
						v => new UpdateAuthorDto { Email = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 5:
				{
					Console.Clear();
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewPhoneNumber,
						v => new UpdateAuthorDto { PhoneNumber = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewBirthDate,
						v => new UpdateAuthorDto { BirthDate = v });
					if (updated is not null) desiredAuthor = updated;
					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("\nEnter new biography");
					var updated = PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewBiography,
						v => new UpdateAuthorDto { Biography = v });
					if (updated is not null) desiredAuthor = updated;
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


	private static AuthorDto? PerformUpdate<T>(AuthorManagementService authorManagementService, int desiredAuthorId,
		T? newValue, Func<T, UpdateAuthorDto> buildDto)
	{
		if (newValue is null) return null;

		var dto = buildDto(newValue);
		var result = authorManagementService.UpdateAuthor(desiredAuthorId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Data;
	}


	private static void SearchAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasAnyPermission(Permission.SearchAuthorForMember, Permission.FullSearchAuthor))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Action<IReadOnlyList<AuthorDto>> printer = authorization.HasAnyPermission(Permission.FullSearchAuthor)
			? author => AuthorPrinter.PrintFullTable(author, "Search Result")
			: author => AuthorPrinter.PrintTable(author, "Search Result");

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING AUTHOR MENU " + new string('=', 36));
			if (authorManagementService.GetAllAuthors().Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableAuthor);
				return;
			}

			var items = new List<(AuthorSearchField? Field, string Label, Permission[] RequiredPermissions)>
			{
				(AuthorSearchField.Name, "Name", [Permission.FullSearchAuthor, Permission.SearchAuthorForMember]),
				(AuthorSearchField.NationalCode, "National Code", [Permission.FullSearchAuthor]),
				(AuthorSearchField.Email, "Email", [Permission.FullSearchAuthor, Permission.SearchAuthorForMember]),
				(AuthorSearchField.PhoneNumber, "Phone Number", [Permission.FullSearchAuthor]),
			};
			var available = items.Where(i =>
				i.RequiredPermissions.Length == 0 || authorization.HasAnyPermission(i.RequiredPermissions)).ToList();

			var displayNumber = 1;
			foreach (var (_, label, _) in available) Console.WriteLine($"{displayNumber++}. {label}");
			Console.WriteLine($"{displayNumber}. Back");

			var choice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, displayNumber);
			if (choice is null) return;

			if (choice == displayNumber) // Back
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Author"));
				return;
			}

			var (field, _, _) = available[choice.Value - 1];
			var prompt = field switch
			{
				AuthorSearchField.Name => Messages.SearchName,
				AuthorSearchField.NationalCode => Messages.SearchNationalCode,
				AuthorSearchField.Email => Messages.SearchEmail,
				AuthorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			PersonHelper.SearchAndDisplay(prompt,
				term => authorManagementService.SearchAuthor(term, field!.Value), printer,
				Messages.NotAuthorMatched);

			ConsoleHelper.Pause();
		}
	}


	private static void SortAuthors(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		if (!authorization.HasAnyPermission(Permission.SortAuthorForMember, Permission.FullSortAuthor))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		var sortFields = GetAvailableAuthorSortFields(authorization);
		while (true)
		{
			Console.Clear();
			var fieldHeaders = new[] { "#", "Sort By" };
			var fieldRows = sortFields
				.Select((f, index) => new string[][] { [(index + 1).ToString()], [f.Label] }).ToList();
			fieldRows.Add([[(sortFields.Count + 1).ToString()], ["Back"]]);


			ConsoleTable.PrintTable("Sort Authors", fieldHeaders, fieldRows);

			var backOption = sortFields.Count + 1;
			var choice = ConsoleHelper.ReadInt(Messages.SortFieldQuestion, 1, backOption);
			if (choice is null) return;

			if (choice == backOption)
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SortCancelled, "Author"));
				return;
			}

			var (field, _) = sortFields[choice.Value - 1];
			var sortDirection = SelectSortDirection();
			if (sortDirection is null) continue;

			var sortedAuthors = authorManagementService.GetAllAuthors(field, sortDirection.Value);
			if (sortedAuthors.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableAuthor);
				continue;
			}

			var sortDescription = $"{field} ({sortDirection})";

			var canViewFullDetails = authorization.HasPermission(Permission.FullSortAuthor);
			if (canViewFullDetails)
				AuthorPrinter.PrintFullTable(sortedAuthors, $"Sorted Authors - {sortDescription}");
			else
				AuthorPrinter.PrintTable(sortedAuthors, $"Sorted Authors - {sortDescription}");

			ConsoleHelper.Pause();
		}
	}


	private static List<(AuthorSortField Field, string Label)> GetAvailableAuthorSortFields(
		IAuthorizationService authorization)
	{
		var fields = new List<(AuthorSortField Field, string Label)>
		{
			(AuthorSortField.Id, "ID"),
			(AuthorSortField.FirstName, "First Name"),
			(AuthorSortField.LastName, "Last Name"),
			(AuthorSortField.FullName, "Full Name"),
			(AuthorSortField.Email, "Email")
		};

		if (!authorization.HasPermission(Permission.FullSortAuthor)) return fields;
		fields.Insert(3, (AuthorSortField.NationalCode, "National Code"));
		fields.Add((AuthorSortField.BirthDate, "Birth Date"));
		fields.Add((AuthorSortField.BookCount, "Book Count"));
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



	private static void ViewAuthorMenu(AuthorManagementService authorManagementService,
		IAuthorizationService authorization, ICurrentUserSession session, LibraryStatisticsService statisticsService)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
			    Permission.ViewAuthorDetails,
			    Permission.ViewAuthorBooks,
			    Permission.ViewAllAuthors))
		{
			return;
		}

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
			switch (ViewAuthorMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAuthorDetails,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
						author => MenuHelper.SelectAuthor(author, authorization), Messages.NotAvailableAuthor);
					if (desiredAuthor is not null) AuthorPrinter.PrintDetails(desiredAuthor);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewBooksByAuthor(authorManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllAuthors,
						    Messages.AccessDenied))
						break;

					Console.Clear();
					var authors = authorManagementService.GetAllAuthors();

					if (authors.Count == 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableAuthor);
					else if (authorization.HasPermission(Permission.ViewAuthorFullDetails))
						AuthorPrinter.PrintFullTable(authors);
					else
						AuthorPrinter.PrintTable(authors);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					ConsoleHelper.ShowInfo(Messages.BackToAuthorMenu);
					return;
				}
			}
		}
	}


	private static int ViewAuthorMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "View Author Details", authorization.HasPermission(Permission.ViewAuthorDetails)),
			(2, "View Author's Books", authorization.HasPermission(Permission.ViewAuthorBooks)),
			(3, "View All Authors", authorization.HasPermission(Permission.ViewAllAuthors)),
			(4, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 35) + " VIEW AUTHOR MENU " + new string('=', 35));

		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
	}


	private static void ViewBooksByAuthor(AuthorManagementService authorManagementService,
		IAuthorizationService authorization)
	{
		if (!SessionGuard.RequirePermission(authorization, Permission.ViewAuthorBooks, Messages.AccessDenied)) return;
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