using LibraryManagementSystem.Common;

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

	public static string? ReadISBN(string prompt)
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

			if (string.IsNullOrWhiteSpace(trimmed))
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


	private static string? GetValidString(string prompt, Func<string, bool> validator, string errorMessage)
	{
		string? input;

		while (true)
		{
			input = ReadString(prompt);

			if (input == null)
				return null;

			if (validator(input))
			{
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(errorMessage);
			Console.ResetColor();
		}

		return input;
	}

	public static string? GetValidName(string prompt, string field)
	{
		return GetValidString(prompt,
			input => Validator.NameValidator(input, ValidationConstants.MinNameLength,
				ValidationConstants.MaxNameLength), $"Invalid {field}. Please try again.");
	}

	public static string? GetValidNationalCode(string prompt)
	{
		return GetValidString(prompt, Validator.NationalCodeValidator, "Invalid national code.Please try again.");
	}

	public static string? GetValidEmail(string prompt)
	{
		return GetValidString(prompt, Validator.EmailValidator, "Invalid email address.Please try again.");
	}

	public static string? GetValidPhoneNumber(string prompt)
	{
		return GetValidString(prompt, Validator.PhoneNumberValidator, "Invalid phone number.Please try again.");
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