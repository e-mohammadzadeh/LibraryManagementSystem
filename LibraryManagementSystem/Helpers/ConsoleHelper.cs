using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using System.Runtime.InteropServices.JavaScript;

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

				ShowInfo($"Please enter a number between {min} and {max}.");
			}
			else
			{
				ShowError(ValidationMessages.InvalidNumber);
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

			ShowError(ValidationMessages.EmptyInput);
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

			ShowError(ValidationMessages.InvalidBirthDateFormat);
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
				ShowError(ValidationMessages.EmptyInput);
				continue;
			}

			if (Validator.ISBNValidator(trimmed))
				return trimmed;

			ShowError(ValidationMessages.InvalidISBN);
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
					ShowError(ValidationMessages.InvalidYesNo);
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

			ShowError(errorMessage);
		}

		return input;
	}


	public static string? GetValidName(string prompt, string field, int minLength, int maxLength)
	{
		return GetValidString(prompt, input => Validator.NameValidator(input, minLength, maxLength),
			$"Invalid {field}. Please try again.");
	}


	public static string? GetValidNationalCode(string prompt)
	{
		return GetValidString(prompt, Validator.NationalCodeValidator, ValidationMessages.InvalidNationalCode);
	}


	public static string? GetValidEmail(string prompt)
	{
		return GetValidString(prompt, Validator.EmailValidator, ValidationMessages.InvalidEmail);
	}


	public static string? GetValidPhoneNumber(string prompt)
	{
		return GetValidString(prompt, Validator.PhoneNumberValidator, ValidationMessages.InvalidPhoneNumber);
	}


	public static DateOnly? GetValidBirthDate(string prompt)
	{
		DateOnly? birthDate;
		while (true)
		{
			birthDate = ReadDateOnly(prompt);

			if (birthDate is null)
				return null;

			if (Validator.BirthDateValidator(birthDate.Value))
			{
				break;
			}

			ShowError(ValidationMessages.InvalidBirthDate);
		}

		return birthDate;
	}


	public static DateOnly? GetValidDate(string prompt)
	{
		DateOnly? date;
		while (true)
		{
			date = ReadDateOnly(prompt);
			if (date is null)
				return null;

			if (Validator.DateValidator(date))
			{
				break;
			}

			ShowError(ValidationMessages.InvalidDate);
		}

		return date;
	}


	public static void ShowResult<T>(ServiceResult<T> result) where T : class
	{
		if (result.Success)
			ConsoleHelper.ShowSuccess(result.Message ?? "Operation completed successfully.");
		else
			ConsoleHelper.ShowError(result.Message ?? "Operation failed.");
	}


	public static void ShowError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(message);
		Console.ResetColor();
	}


	public static void ShowSuccess(string message)
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(message);
		Console.ResetColor();
	}


	public static void ShowWarning(string message)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(message);
		Console.ResetColor();
	}


	public static void ShowInfo(string message)
	{
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine(message);
		Console.ResetColor();
	}
}