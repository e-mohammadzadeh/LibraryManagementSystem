using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

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
					AddAuthor(userManagementService);
					break;
				}
				case 2:
				{
					EditAuthor(userManagementService);
					break;
				}
				case 3:
				{
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
					var desiredAuthor = SelectExistingAuthor(userManagementService);
					if (desiredAuthor == null)
						return;

					DisplayAuthorDetails(desiredAuthor);
					break;
				}
				case 6:
				{
					if (userManagementService.GetAllAuthors().Count == 0)
						ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);

					DisplayAuthors(userManagementService.GetAllAuthors());
					Console.WriteLine("\nPress any key to continue...");
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(3000);
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
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7)
			{
				return result;
			}

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
		}
	}


	private static void AddAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING AUTHOR MENU ============================");

		var firstName = ConsoleHelper.GetValidName("Enter author's first name: ", "first name",
			ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

		if (firstName == null)
			return;

		var lastName = ConsoleHelper.GetValidName("Enter author's last name: ", "last name",
			ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

		if (lastName == null)
			return;

		var nationalCode = ConsoleHelper.GetValidNationalCode("Enter author's national code: ");
		if (nationalCode == null)
			return;

		var email = ConsoleHelper.GetValidEmail("Enter author's email: ");
		if (email == null)
			return;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter author's phone number: ");
		if (phoneNumber == null)
			return;

		var birthDate = ConsoleHelper.GetValidBirthDate("Enter author's birth date (yyyy-MM-dd): ");
		if (birthDate == null)
			return;

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional): ", true);

		var result = userManagementService.AddAuthor(firstName, lastName, nationalCode, email, phoneNumber,
			birthDate.Value, biography);

		ShowResult(result);
	}


	private static void EditAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING AUTHOR MENU ============================");
		var desiredAuthor = SelectExistingAuthor(userManagementService);
		if (desiredAuthor == null)
			return;

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
			if (editMenuChoice == null)
				return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var authorNewFirstName = ConsoleHelper.GetValidName("Enter new first name", "first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewFirstName,
						    v => new UpdateAuthorDto { FirstName = v }))
						return;

					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("Enter new last name: ", "last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewLastName,
						    v => new UpdateAuthorDto { LastName = v }))
						return;

					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code: ");
					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewNationalCode,
						    v => new UpdateAuthorDto { NationalCode = v }))
						return;

					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("Enter new email: ");
					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewEmail,
						    v => new UpdateAuthorDto { Email = v }))
						return;

					break;
				}
				case 5:
				{
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number: ");
					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewPhoneNumber,
						    v => new UpdateAuthorDto { PhoneNumber = v }))
						return;

					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date (yyyy-MM-dd): ");
					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewBirthDate,
						    v => new UpdateAuthorDto { BirthDate = v }))
						return;

					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("Enter new biography: ");
					if (!PerformUpdate(userManagementService, desiredAuthor.AuthorId, authorNewBiography,
						    v => new UpdateAuthorDto { Biography = v }))
						return;

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
			if (choice == true)
			{
				Console.Clear();
				continue;
			}

			Console.Clear();
			return;
		}
	}


	private static void RemoveAuthor(UserManagementService userManagementService)
	{
		// TODO	Implement SOFT DELETE system with flags like `IsDeleted = true` or `IsActive = False`
		Console.WriteLine("============================ REMOVING AUTHOR MENU ============================");
		var desiredAuthor = SelectExistingAuthor(userManagementService);
		if (desiredAuthor == null)
			return;

		DisplayAuthorDetails(desiredAuthor);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredAuthor.FirstName} {desiredAuthor.LastName}");

		if (choice != true)
			return;

		var result = userManagementService.RemoveAuthor(desiredAuthor.AuthorId);
		ShowResult(result);
	}


	private static void SearchAuthor(UserManagementService userManagementService)
	{
		while (true)
		{
			Console.WriteLine("============================ SEARCHING AUTHOR MENU ============================");
			var authorsList = userManagementService.GetAllAuthors();
			if (authorsList.Count == 0)
			{
				ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 5);
			if (searchMenuChoice == null)
				return;

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
					ConsoleHelper.ShowError("Search cancelled. Returning to Author Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey(true);
		}
	}



	private static bool PerformUpdate<T>(UserManagementService userManagementService, int desiredAuthorId, T? newValue,
		Func<T, UpdateAuthorDto> buildDto)
	{
		if (newValue is null)
			return false;

		var dto = buildDto(newValue);
		var result = userManagementService.UpdateAuthor(desiredAuthorId, dto);
		ShowResult(result);
		return true;
	}


	private static void SearchAuthorsAndDisplay(UserManagementService userManagementService, string prompt,
		Func<Author, string?> selector)
	{
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem == null)
			return;

		var result = userManagementService.SearchAuthors(searchItem, selector);

		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning("No authors matched your search.");
			return;
		}

		DisplayAuthors(result);
	}


	private static void ShowResult(ServiceResult<Author> result)
	{
		if (result.Success)
			ConsoleHelper.ShowSuccess(result.Message ?? "Operation completed successfully.");
		else
			ConsoleHelper.ShowError(result.Message ?? "Operation failed.");
	}


	private static Author? SelectExistingAuthor(UserManagementService userManagementService)
	{
		var authorList = userManagementService.GetAllAuthors();
		if (authorList.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
			return null;
		}

		return SelectAuthor(authorList);
	}


	private static Author? SelectAuthor(IReadOnlyList<Author> authorsList)
	{
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
			return null;
		}

		while (true)
		{
			DisplayAuthors(authorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredAuthorId = ConsoleHelper.ReadInt("Enter the number of the author you wish to edit", 1,
				authorsList.Last().AuthorId);

			if (desiredAuthorId == null)
				return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.AuthorId == desiredAuthorId.Value);

			if (desiredAuthor != null)
				return desiredAuthor;

			ConsoleHelper.ShowError("Author not found. Please try again.");
		}
	}



	private static void DisplayAuthors(IReadOnlyList<Author> authors)
	{
		if (authors.Count == 0)
			return;

		Console.WriteLine("{0,3} {1, 20} {2, 25}", "ID", "Author Name", "Email Address");
		Console.WriteLine("========================================================");

		foreach (var author in authors)
		{
			var fullName = author.FirstName + " " + author.LastName;
			Console.WriteLine("{0,3} {1, 20} {2, 25}", author.AuthorId, fullName, author.Email);
		}

		Console.WriteLine("========================================================");
	}


	private static void DisplayAuthorDetails(Author author)
	{
		Console.WriteLine("Author Details:");

		Console.WriteLine("{0, -20} [{1} {2}]", "Name:", author.FirstName, author.LastName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", author.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", author.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", author.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", author.BirthDate);
		Console.WriteLine("{0, -20} [{1}]", "Biography:", author.Biography);
		Console.WriteLine("{0, -20} [{1} {2}]", "Books:", author.Books.Count, "associated books");
	}
}