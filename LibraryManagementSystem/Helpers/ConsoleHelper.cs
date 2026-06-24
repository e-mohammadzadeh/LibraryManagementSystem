namespace LibraryManagementSystem.Helpers;

// This static class loop until valid input is received.
public static class ConsoleHelper
{
	public static int? ReadInt(string prompt, int min, int max)
	{
		while (true)
		{
			Console.Write($"{prompt} (or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
				return null;

			if (int.TryParse(input, out var value))
			{
				if (value >= min && value <= max)
					return value;

				Console.WriteLine($"Please enter a number between {min} and {max}.");
			}
			else
			{
				Console.WriteLine("Invalid number. Please try again");
			}
		}
	}

	public static string? ReadString(string prompt, bool allowEmpty = false)
	{
		while (true)
		{
			Console.Write($"{prompt} (or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			if (!string.IsNullOrWhiteSpace(trimmed) || allowEmpty)
				return trimmed;

			Console.WriteLine("Input cannot be empty. Please try again.");
		}
	}

	public static DateOnly? ReadDateOnly(string prompt)
	{
		while (true)
		{
			Console.Write($"{prompt} (YYYY-MM-DD, or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
				return null;

			if (DateOnly.TryParse(input, out var date))
				return date;

			Console.WriteLine("Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).");
		}
	}

	public static string? ReadISBN(string prompt, bool allowEmpty = false)
	{
		while (true)
		{
			Console.Write($"{prompt} ('978-92-95055-02-5', or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(trimmed) && !allowEmpty)
			{
				Console.WriteLine("Input cannot be empty. Please try again.");
				continue;
			}

			if (Validator.ISBNValidator(trimmed))
				return trimmed;

			Console.WriteLine("Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.");
		}
	}

	public static bool? ReadYesNo(string prompt)
	{
		while (true)
		{
			Console.Write($"{prompt} (y/n, or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim().ToLower();
			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase))
				return null;

			switch (trimmed)
			{
				case "y" or "yes":
					return true;
				case "n" or "no":
					return false;
				default:
					Console.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");
					break;
			}
		}
	}

	public static string? GetValidFirstName(string prompt, bool allowEmpty = false)
	{
		string? firstName;
		const int minNameLength = 2;
		const int maxNameLength = 50;
		while (true)
		{
			firstName = ReadString(prompt, allowEmpty);
			if (firstName == null)
				return null;

			if (Validator.NameValidator(firstName, minNameLength, maxNameLength))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid first name.Please try again.");
			Console.ResetColor();
		}

		return firstName;
	}

	public static string? GetValidLastName(string prompt, bool allowEmpty = false)
	{
		string? lastName;
		const int minNameLength = 2;
		const int maxNameLength = 50;
		while (true)
		{
			lastName = ReadString(prompt, allowEmpty);
			if (lastName == null)
				return null;

			if (Validator.NameValidator(lastName, minNameLength, maxNameLength))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid last name.Please try again.");
			Console.ResetColor();
		}

		return lastName;
	}

	public static string? GetValidNationalCode(string prompt)
	{
		string? nationalCode;

		while (true)
		{
			nationalCode = ReadString(prompt);

			if (nationalCode == null)
				return null;

			if (Validator.NationalCodeValidator(nationalCode))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid national code.Please try again.");
			Console.ResetColor();
		}

		return nationalCode;
	}

	public static string? GetValidEmail(string prompt)
	{
		string? email;
		while (true)
		{
			email = ReadString(prompt);

			if (email == null)
				return null;

			if (Validator.EmailValidator(email))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid email address.Please try again.");
			Console.ResetColor();
		}

		return email;
	}

	public static string? GetValidPhoneNumber(string prompt)
	{
		string? phoneNumber;
		while (true)
		{
			phoneNumber = ReadString(prompt);

			if (phoneNumber == null)
				return null;

			if (Validator.PhoneNumberValidator(phoneNumber))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid phone number.Please try again.");
			Console.ResetColor();
		}

		return phoneNumber;
	}

	public static DateOnly? GetValidBirthDate(string prompt)
	{
		DateOnly? birthDate;
		while (true)
		{
			birthDate = ReadDateOnly(prompt);

			if (birthDate == null)
				return null;

			if (Validator.BirthDateValidator(birthDate.Value))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid birth date. Please try again.");
			Console.ResetColor();
		}

		return birthDate;
	}
}