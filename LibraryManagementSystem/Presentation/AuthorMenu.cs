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
					ViewAllAuthors(userManagementService);
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
		string? firstName;
		string? lastName;
		string? nationalCode;
		string? email;
		string? phoneNumber;
		DateOnly? birthDate;

		while (true) // Validate First Name
		{
			firstName = ConsoleHelper.ReadString("Enter author's first name: ");
			if (firstName == null)
				return;

			if (Validator.NameValidator(firstName, 2, 50))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid first name.Please try again.");
			Console.ResetColor();
		}

		while (true) // Validate Last Name
		{
			lastName = ConsoleHelper.ReadString("Enter author's last name: ");
			if (lastName == null)
				return;

			if (Validator.NameValidator(lastName, 2, 50))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid last name.Please try again.");
			Console.ResetColor();
		}

		while (true) // Validate National Code
		{
			nationalCode = ConsoleHelper.ReadString("Enter author's national code: ");

			if (nationalCode == null)
				return;

			if (Validator.NationalCodeValidator(nationalCode))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid national code.Please try again.");
			Console.ResetColor();
		}

		while (true) // Validate Email
		{
			email = ConsoleHelper.ReadString("Enter author's email: ");

			if (email == null)
				return;

			if (Validator.EmailValidator(email))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid email address.Please try again.");
			Console.ResetColor();
		}

		while (true) // Validate Phone Number
		{
			phoneNumber = ConsoleHelper.ReadString("Enter author's phone number: ");

			if (phoneNumber == null)
				return;

			if (Validator.PhoneNumberValidator(phoneNumber))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid phone number.Please try again.");
			Console.ResetColor();
		}

		while (true) // Validate Birth Date
		{
			birthDate = ConsoleHelper.ReadDateOnly("Enter author's birth date (yyyy-MM-dd): ");

			if (birthDate == null)
				return;

			if (Validator.BirthDateValidator(birthDate.Value))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid birth date. Please try again.");
			Console.ResetColor();
		}

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional): ", true);

		var result = userManagementService.AddAuthor(firstName, lastName, nationalCode, email, phoneNumber,
			birthDate.Value, biography);

		if (result.Success)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Author added successfully.");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Failed to add author." + result.Message);
		}

		Console.ResetColor();
	}

	private static void EditAuthor(UserManagementService userManagementService)
	{
		var authorsList = userManagementService.GetAllAuthors();
		if (authorsList.Count == 0)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("No authors found. First add new author.");
			Console.ResetColor();
			return;
		}

		ViewAllAuthors(userManagementService);
		var desiredAuthorId = ConsoleHelper.ReadInt("Which Author do you want to edit (Enter just ID)?", 1,
			authorsList.Count);

		if (desiredAuthorId == null)
			return;

		var desiredAuthor = userManagementService.FindAuthorById(desiredAuthorId.Value);

		if (desiredAuthor == null)
			return;

		Console.WriteLine("1. First Name\t\t[{0}]", desiredAuthor.FirstName);
		Console.WriteLine("2. Last Name\t\t[{0}]", desiredAuthor.LastName);
		Console.WriteLine("3. National Code\t[{0}]", desiredAuthor.NationalCode);
		Console.WriteLine("4. Email\t\t[{0}]", desiredAuthor.Email);
		Console.WriteLine("5. Phone Number\t[{0}]", desiredAuthor.PhoneNumber);
		Console.WriteLine("6. Birth Date\t\t[{0}]", desiredAuthor.BirthDate);
		Console.WriteLine("7. Biography\t\t[{0}]", desiredAuthor.Biography);
		Console.WriteLine("8. Cancel");
		var editMenuChoice = ConsoleHelper.ReadInt("Which field do you want to edit (Enter a number)?", 1, 8);

		switch (editMenuChoice)
		{
			case 1:
			{
				string? authorNewFirstName;
				while (true) // Validate First Name
				{
					authorNewFirstName = ConsoleHelper.ReadString("Enter new first name: ");
					if (authorNewFirstName == null)
						return;

					if (Validator.NameValidator(authorNewFirstName, 2, 50))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid first name.Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, authorNewFirstName, null, null, null,
					null, null, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 2:
			{
				string? authorNewLastName;
				while (true) // Validate Last Name
				{
					authorNewLastName = ConsoleHelper.ReadString("Enter new last name: ");
					if (authorNewLastName == null)
						return;

					if (Validator.NameValidator(authorNewLastName, 2, 50))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid last name.Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, null, authorNewLastName, null, null,
					null,
					null, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 3:
			{
				string? authorNewNationalCode;
				while (true) // Validate National Code
				{
					authorNewNationalCode = ConsoleHelper.ReadString("Enter new national code: ");

					if (authorNewNationalCode == null)
						return;

					if (Validator.NationalCodeValidator(authorNewNationalCode))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid national code.Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, null, null, authorNewNationalCode,
					null,
					null, null, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 4:
			{
				string? authorNewEmail;
				while (true) // Validate Email
				{
					authorNewEmail = ConsoleHelper.ReadString("Enter new email: ");

					if (authorNewEmail == null)
						return;

					if (Validator.EmailValidator(authorNewEmail))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid email address.Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, null, null, null, authorNewEmail,
					null,
					null, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 5:
			{
				string? authorNewPhoneNumber;
				while (true) // Validate Phone Number
				{
					authorNewPhoneNumber = ConsoleHelper.ReadString("Enter new phone number: ");

					if (authorNewPhoneNumber == null)
						return;

					if (Validator.PhoneNumberValidator(authorNewPhoneNumber))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid phone number.Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, null, null, null, null,
					authorNewPhoneNumber,
					null, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 6:
			{
				DateOnly? authorNewBirthDate;
				while (true) // Validate Birth Date
				{
					authorNewBirthDate = ConsoleHelper.ReadDateOnly("Enter new birth date (yyyy-MM-dd): ");

					if (authorNewBirthDate == null)
						return;

					if (Validator.BirthDateValidator(authorNewBirthDate.Value))
					{
						break;
					}

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid birth date. Please try again.");
					Console.ResetColor();
				}

				var result = userManagementService.UpdateAuthor(desiredAuthor, null, null, null, null, null,
					authorNewBirthDate, null);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
				break;
			}
			case 7:
			{
				var authorNewBiography = ConsoleHelper.ReadString("Enter new biography: ");
				var result = userManagementService.UpdateAuthor(desiredAuthor, null, null, null, null, null,
					null, authorNewBiography);

				if (result.Success)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Author updated successfully.");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to update author.");
				}

				Console.ResetColor();
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
	}


	private static void ViewAllAuthors(UserManagementService userManagementService)
	{
		var authors = userManagementService.GetAllAuthors();
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
}