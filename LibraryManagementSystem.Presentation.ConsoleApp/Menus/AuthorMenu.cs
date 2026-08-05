using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class AuthorMenu
{
	public static void AuthorMenuController(AuthorManagementService authorManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session)
	{
		if (!SessionGuard.RequireAuthorManagement(session)) return;

		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(ValidationMessages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (AuthorMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					AddAuthor(authorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					EditAuthor(authorManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveAuthor(authorManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					SearchAuthor(authorManagementService);
					break;
				}
				case 5:
				{
					Console.Clear();
					var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(),
						MenuHelper.SelectAuthor,
						ValidationMessages.NotAvailableAuthor);
					if (desiredAuthor is not null)
					{
						AuthorPrinter.PrintDetails(desiredAuthor);
						ConsoleHelper.Pause();
					}

					break;
				}
				case 6:
				{
					Console.Clear();
					if (authorManagementService.GetAllAuthors().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
					else
						AuthorPrinter.PrintTable(authorManagementService.GetAllAuthors());
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(ValidationMessages.Back2MainMenu);
					ConsoleHelper.Pause();
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int AuthorMenuList(ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Add Author", session.IsAdmin || session.IsLibrarian),
			(2, "Edit Author", session.IsAdmin || session.IsLibrarian),
			(3, "Remove Author", session.IsAdmin),
			(4, "Search Author", session.IsAdmin || session.IsLibrarian),
			(5, "View Author Details", session.IsAdmin || session.IsLibrarian),
			(6, "View All Authors", session.IsAdmin || session.IsLibrarian),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 35) + " AUTHOR MENU " + new string('=', 35));

			var displayNumber = 1;
			foreach (var item in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {item.DisplayText}");
				displayNumber++;
			}

			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count)
				return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
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


	private static void EditAuthor(AuthorManagementService authorManagementService)
	{
		Console.WriteLine(new string('=', 36) + " EDITING AUTHOR MENU " + new string('=', 36));
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(), MenuHelper.SelectAuthor,
			ValidationMessages.NotAvailableAuthor);
		if (desiredAuthor is null) return;

		while (true)
		{
			Console.WriteLine("\n{0, -20} [{1}]", "1. First Name", desiredAuthor.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredAuthor.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredAuthor.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredAuthor.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredAuthor.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredAuthor.BirthDate);
			Console.WriteLine("{0, -20} [{1}]", "7. Biography", desiredAuthor.Biography);
			Console.WriteLine("8. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.EditMenuQuestion, 1, 8);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var authorNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewFirstName,
						v => new UpdateAuthorDto { FirstName = v });

					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewLastName,
						v => new UpdateAuthorDto { LastName = v });

					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewNationalCode,
						v => new UpdateAuthorDto { NationalCode = v });

					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewEmail,
						v => new UpdateAuthorDto { Email = v });

					break;
				}
				case 5:
				{
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewPhoneNumber,
						v => new UpdateAuthorDto { PhoneNumber = v });

					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewBirthDate,
						v => new UpdateAuthorDto { BirthDate = v });

					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("Enter new biography");
					PerformUpdate(authorManagementService, desiredAuthor.Id, authorNewBiography,
						v => new UpdateAuthorDto { Biography = v });

					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(string.Format(ValidationMessages.EditCancelled, "Author"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(ValidationMessages.EditContinuesQuestion);
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static void RemoveAuthor(AuthorManagementService authorManagementService, ICurrentUserSession session)
	{
		if (!session.IsAdmin)
		{
			ConsoleHelper.ShowError(ValidationMessages.AccessDenied);
			return;
		}

		// TODO	(SQL Server)	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine(new string('=', 36) + " REMOVING AUTHOR MENU " + new string('=', 36));
		var desiredAuthor = MenuHelper.SelectExisting(authorManagementService.GetAllAuthors(), MenuHelper.SelectAuthor,
			ValidationMessages.NotAvailableAuthor);

		PersonHelper.PerformRemove(desiredAuthor, desiredAuthor?.FirstName ?? "", desiredAuthor?.LastName ?? "",
			AuthorPrinter.PrintDetails, () => authorManagementService.RemoveAuthor(desiredAuthor!.Id));
	}


	private static void SearchAuthor(AuthorManagementService authorManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING AUTHOR MENU " + new string('=', 36));
			var authorsList = authorManagementService.GetAllAuthors();
			if (authorsList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
				ConsoleHelper.Pause();
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.SearchMenuQuestion, 1, 5);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					PersonHelper.SearchAndDisplay("Enter a name to search",
						term => authorManagementService.SearchAuthor(term, AuthorSearchField.Name), AuthorPrinter.PrintTable,
						ValidationMessages.NotAuthorMatched);

					break;
				}
				case 2:
				{
					PersonHelper.SearchAndDisplay("Enter a national code to search",
						term => authorManagementService.SearchAuthor(term, AuthorSearchField.NationalCode),
						AuthorPrinter.PrintTable, ValidationMessages.NotAuthorMatched);

					break;
				}
				case 3:
				{
					PersonHelper.SearchAndDisplay("Enter an email to search",
						term => authorManagementService.SearchAuthor(term, AuthorSearchField.Email),
						AuthorPrinter.PrintTable, ValidationMessages.NotAuthorMatched);

					break;
				}
				case 4:
				{
					PersonHelper.SearchAndDisplay("Enter a phone number to search",
						term => authorManagementService.SearchAuthor(term, AuthorSearchField.PhoneNumber),
						AuthorPrinter.PrintTable, ValidationMessages.NotAuthorMatched);
					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo(string.Format(ValidationMessages.SearchCancelled, "Author"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static void PerformUpdate<T>(AuthorManagementService authorManagementService, int desiredAuthorId,
		T? newValue,
		Func<T, UpdateAuthorDto> buildDto)
	{
		if (newValue is null) return;

		var dto = buildDto(newValue);
		var result = authorManagementService.UpdateAuthor(desiredAuthorId, dto);
		ConsoleHelper.ShowResult(result);
	}
}