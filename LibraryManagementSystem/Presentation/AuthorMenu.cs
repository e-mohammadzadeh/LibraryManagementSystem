using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public class AuthorMenu
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
			switch (AuthorMenuList())
			{
				case 1:
				{
					AddAuthor(userManagementService);
					break;
				}
				case 2:
				{
					break;
				}
				case 3:
				{
					break;
				}
				case 4:
				{
					break;
				}
				case 5:
				{
					break;
				}
				case 6:
				{
					ViewAllAuthors(userManagementService);
					break;
				}
				case 7:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Backing to main menu...\n");
					Console.ResetColor();
					continueProgram = false;
					break;
				}
			}
		}
	}

	private static void AddAuthor(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING AUTHOR MENU ============================");

		var firstName = ConsoleHelper.ReadString("Enter author's first name: ");
		var lastName = ConsoleHelper.ReadString("Enter author's last name: ");
		string? nationalCode;
		string? email;
		string? phoneNumber;
		DateOnly? birthDate;

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
			email = ConsoleHelper.ReadString("Enter author email: ");

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
			phoneNumber = ConsoleHelper.ReadString("Enter author phone number: ");

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
			birthDate = ConsoleHelper.ReadDateOnly("Enter author birth date (yyyy-MM-dd): ");

			if (birthDate == null)
				return;

			if (Validator.BirthDateValidator(birthDate))
			{
				break;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid birth date. Please try again.");
			Console.ResetColor();
		}

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional): ");

		var result = userManagementService.AddAuthor(firstName, lastName, nationalCode, email, phoneNumber, birthDate,
			biography);

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

	public static void ViewAllAuthors(UserManagementService userManagementService)
	{
		var a = 2 + 2;
	}
}