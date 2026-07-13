using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Printers;

namespace LibraryManagementSystem.Presentation;

public static class AuthorMenu
{
	public static void AuthorMenuController(UserManagementService userManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			switch (AuthorMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddAuthor(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					EditAuthor(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveAuthor(userManagementService);
					break;
				}
				case 4:
				{
					SearchAuthor(userManagementService);
					break;
				}
				case 5:
				{
					Console.Clear();
					var desiredAuthor = SelectExistingAuthor(userManagementService);
					if (desiredAuthor is not null)
					{
						AuthorPrinter.PrintDetails(desiredAuthor);
						ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
						Console.ReadKey(true);
					}

					break;
				}
				case 6:
				{
					Console.Clear();
					if (userManagementService.GetAllAuthors().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
					else
						AuthorPrinter.PrintTable(userManagementService.GetAllAuthors());

					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int AuthorMenuList()
	{
		while (true)
		{
			Console.WriteLine("============================ AUTHOR MENU ============================");
			Console.WriteLine("1. Add Author");
			Console.WriteLine("2. Edit Author");
			Console.WriteLine("3. Remove Author");
			Console.WriteLine("4. Search Author");
			Console.WriteLine("5. View Author Details");
			Console.WriteLine("6. View All Authors");
			Console.WriteLine("7. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	public static CreateAuthorDto? PromptForAuthorDto()
	{
		var firstName = ConsoleHelper.GetValidName("Enter author's first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (firstName == null) return null;

		var lastName = ConsoleHelper.GetValidName("Enter author's last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (lastName == null) return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode("Enter author's national code");
		if (nationalCode == null) return null;

		var email = ConsoleHelper.GetValidEmail("Enter author's email");
		if (email == null) return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter author's phone number");
		if (phoneNumber == null) return null;

		var birthDate = ConsoleHelper.GetValidBirthDate("Enter author's birth date");
		if (birthDate == null) return null;

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional)", true);

		return new CreateAuthorDto
		{
			FirstName = firstName, LastName = lastName, NationalCode = nationalCode, Email = email,
			PhoneNumber = phoneNumber, BirthDate = birthDate.Value, Biography = biography
		};
	}


	private static void AddAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING AUTHOR MENU ============================");
		var authorDto = PromptForAuthorDto();
		if (authorDto is null) return;

		var result = userManagementService.AddAuthor(authorDto);
		ConsoleHelper.ShowResult(result);
	}


	private static void EditAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING AUTHOR MENU ============================");
		var desiredAuthor = SelectExistingAuthor(userManagementService);
		if (desiredAuthor is null) return;

		while (true)
		{
			Console.WriteLine("{0, -20} [{1}]", "1. First Name", desiredAuthor.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredAuthor.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredAuthor.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredAuthor.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredAuthor.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredAuthor.BirthDate);
			Console.WriteLine("{0, -20} [{1}]", "7. Biography", desiredAuthor.Biography);
			Console.WriteLine("8. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt("Enter the number of the field you wish to edit", 1, 8);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var authorNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewFirstName,
						v => new UpdateAuthorDto { FirstName = v });

					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewLastName,
						v => new UpdateAuthorDto { LastName = v });

					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewNationalCode,
						v => new UpdateAuthorDto { NationalCode = v });

					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewEmail,
						v => new UpdateAuthorDto { Email = v });

					break;
				}
				case 5:
				{
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewPhoneNumber,
						v => new UpdateAuthorDto { PhoneNumber = v });

					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewBirthDate,
						v => new UpdateAuthorDto { BirthDate = v });

					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("Enter new biography");
					PerformUpdate(userManagementService, desiredAuthor.Id, authorNewBiography,
						v => new UpdateAuthorDto { Biography = v });

					break;
				}
				case 8:
				{
					ConsoleHelper.ShowError("Edit cancelled. Returning to Author Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo("Do you want to edit another field");
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static void RemoveAuthor(UserManagementService userManagementService)
	{
		// TODO	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine("============================ REMOVING AUTHOR MENU ============================");
		var desiredAuthor = SelectExistingAuthor(userManagementService);
		if (desiredAuthor == null)
		{
			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
			return;
		}

		AuthorPrinter.PrintDetails(desiredAuthor);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredAuthor.FirstName} {desiredAuthor.LastName}");

		if (choice != true) return;
		var result = userManagementService.RemoveAuthor(desiredAuthor.Id);
		ConsoleHelper.ShowResult(result);
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}


	private static void SearchAuthor(UserManagementService userManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("============================ SEARCHING AUTHOR MENU ============================");
			var authorsList = userManagementService.GetAllAuthors();
			if (authorsList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 5);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchAuthorsAndDisplay(userManagementService, "Enter a name to search",
						author => $"{author.FirstName} {author.LastName}");

					break;
				}
				case 2:
				{
					SearchAuthorsAndDisplay(userManagementService, "Enter a national code to search",
						author => author.NationalCode);

					break;
				}
				case 3:
				{
					SearchAuthorsAndDisplay(userManagementService, "Enter an email to search", author => author.Email);
					break;
				}
				case 4:
				{
					SearchAuthorsAndDisplay(userManagementService, "Enter a phone number to search",
						author => author.PhoneNumber);

					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo("Search cancelled. Returning to Author Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
		}
	}



	private static void PerformUpdate<T>(UserManagementService userManagementService, int desiredAuthorId, T? newValue,
		Func<T, UpdateAuthorDto> buildDto)
	{
		if (newValue is null) return;

		var dto = buildDto(newValue);
		var result = userManagementService.UpdateAuthor(desiredAuthorId, dto);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchAuthorsAndDisplay(UserManagementService userManagementService, string prompt,
		Func<Author, string?> selector)
	{
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem == null) return;

		var result = userManagementService.SearchAuthor(searchItem, selector);

		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotAuthorMatched);
			return;
		}

		AuthorPrinter.PrintTable(result);
	}


	private static Author? SelectExistingAuthor(UserManagementService userManagementService)
	{
		var authors = userManagementService.GetAllAuthors();
		if (authors.Count != 0) return MenuHelper.SelectAuthor(authors);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
		return null;
	}
}