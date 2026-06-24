using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public static class AuthorMenu
{
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

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid selection, Try again.\n");
			Console.ResetColor();
		}
	}

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
					break;
				}
				case 2:
				{
					Console.Clear();
					EditAuthor(userManagementService);
					break;
				}
				case 3:
				{
					Console.Clear();
					break;
				}
				case 4:
				{
					Console.Clear();
					break;
				}
				case 5:
				{
					Console.Clear();
					break;
				}
				case 6:
				{
					Console.Clear();
					ViewAllAuthors(userManagementService.GetAllAuthors());
					Console.WriteLine("\nPress any key to continue...");
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Backing to main menu...\n");
					Console.ResetColor();
					Thread.Sleep(3000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}

	private static void AddAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING AUTHOR MENU ============================");

		var firstName = ConsoleHelper.GetValidFirstName("Enter author's first name: ");
		if (firstName == null)
			return;

		var lastName = ConsoleHelper.GetValidLastName("Enter author's last name: ");
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

		ShowResult(result, "Author added successfully.", "Failed to add author.");
	}

	private static void EditAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING AUTHOR MENU ============================");
		var authorsList = userManagementService.GetAllAuthors();
		if (authorsList.Count == 0)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("No authors found. First add new author.");
			Console.ResetColor();
			return;
		}
		
		var desiredAuthor = SelectAuthor(authorsList);
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
			var editMenuChoice = ConsoleHelper.ReadInt("Which field do you want to edit (Enter a number)?", 1, 8);
			if (editMenuChoice == null)
				return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var authorNewFirstName = ConsoleHelper.GetValidFirstName("Enter new first name: ");

					// TODO	UpdateAuthor() method has much more parameters and should be replaced by DTOs
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, authorNewFirstName, null,
						null, null, null, null, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidLastName("Enter new last name: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, authorNewLastName,
						null, null, null, null, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null,
						authorNewNationalCode, null, null, null, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("Enter new email: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null,
						authorNewEmail, null, null, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 5:
				{
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null,
						authorNewPhoneNumber, null, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date (yyyy-MM-dd): ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null, null,
						authorNewBirthDate, null);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("Enter new biography: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null, null,
						null, authorNewBiography);

					ShowResult(result, "Author updated successfully.", "Failed to update author.");
					break;
				}
				case 8:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Edit cancelled. Returning to Author Menu...");
					Console.ResetColor();
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

	private static void ShowResult(ServiceResult<Author> result, string successMessage, string failureMessage)
	{
		if (result.Success)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(successMessage);
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"{failureMessage} {result.Message}");
		}

		Console.ResetColor();
	}


	private static void ViewAllAuthors(IReadOnlyList<Author> authors)
	{
		if (authors.Count == 0)
		{
			Console.WriteLine("No authors found. First add new author.");
		}
		else
		{
			Console.WriteLine("{0,3} {1, 15} {2, 20}", "ID", "Author Name", "Email Address");
			Console.WriteLine("========================================================");

			foreach (var author in authors)
			{
				var fullName = author.FirstName + " " + author.LastName;
				Console.WriteLine("{0,3} {1, 15} {2, 20}", author.AuthorId, fullName, author.Email);
			}

			Console.WriteLine("========================================================");
		}
	}


	private static Author? SelectAuthor(IReadOnlyList<Author> authorsList)
	{
		ViewAllAuthors(authorsList);

		var desiredAuthorId = ConsoleHelper.ReadInt("Which Author do you want to edit (Enter just ID)?", 1,
			authorsList.Last().AuthorId);

		if (desiredAuthorId == null)
			return null;

		var desiredAuthor = authorsList.FirstOrDefault(a => a.AuthorId == desiredAuthorId.Value);

		if (desiredAuthor != null) 
			return desiredAuthor;

		ServiceResult<Author>.Fail("Author not found. Please try again.");
		return null;
	}
}