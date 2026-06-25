using System.ComponentModel.DataAnnotations;
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

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
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
					RemoveAuthor(userManagementService);
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
					ViewAuthorDetails(Author author);
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
					ConsoleHelper.ShowError("Backing to main menu...\n");
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

		ShowResult(result, "Author added successfully.", "Failed to add author.");
	}

	private static void EditAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING AUTHOR MENU ============================");
		var authorsList = userManagementService.GetAllAuthors();
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError("No authors found. First add new author.");
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
					var authorNewFirstName = ConsoleHelper.GetValidName("Enter new first name: ", "first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					// TODO	UpdateAuthor() method has much more parameters and should be replaced by DTOs
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, authorNewFirstName, null,
						null, null, null, null, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 2:
				{
					var authorNewLastName = ConsoleHelper.GetValidName("Enter new last name: ", "last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, authorNewLastName,
						null, null, null, null, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 3:
				{
					var authorNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null,
						authorNewNationalCode, null, null, null, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 4:
				{
					var authorNewEmail = ConsoleHelper.GetValidEmail("Enter new email: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null,
						authorNewEmail, null, null, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 5:
				{
					var authorNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null,
						authorNewPhoneNumber, null, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 6:
				{
					var authorNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date (yyyy-MM-dd): ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null,
						null,
						authorNewBirthDate, null);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
					break;
				}
				case 7:
				{
					var authorNewBiography = ConsoleHelper.ReadString("Enter new biography: ");
					var result = userManagementService.UpdateAuthor(desiredAuthor.AuthorId, null, null, null, null,
						null,
						null, authorNewBiography);

					ShowResult(result, ValidationMessages.SuccessUpdate, ValidationMessages.FailureUpdate);
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
		var authorsList = userManagementService.GetAllAuthors();
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError("No authors found. First add new author.");
			return;
		}

		var desiredAuthor = SelectAuthor(authorsList);
		if (desiredAuthor == null)
			return;

		ViewAuthorDetails(desiredAuthor);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredAuthor.FirstName} {desiredAuthor.LastName}");

		if (choice != true)
			return;

		var result = userManagementService.RemoveAuthor(desiredAuthor);
		ShowResult(result, ValidationMessages.SuccessRemove, ValidationMessages.FailureRemove);
	}

	private static void ShowResult(ServiceResult<Author> result, string successMessage, string failureMessage)
	{
		if (result.Success)
			ConsoleHelper.ShowSuccess(successMessage);
		else
			ConsoleHelper.ShowError($"{failureMessage} {result.Message}");
	}


	private static void ViewAllAuthors(IReadOnlyList<Author> authors)
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError("No authors found. First add new author.");
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


	private static void ViewAuthorDetails(Author author)
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