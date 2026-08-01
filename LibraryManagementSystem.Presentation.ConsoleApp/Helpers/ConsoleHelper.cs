using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Application.Validators;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

// This static class loop until valid input is received.
public static class ConsoleHelper
{
	public static int? ReadInt(string prompt, int min, int max)
	{
		while (true)
		{
			Console.Write($"{prompt} (type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;
			if (int.TryParse(input, out var value))
			{
				if (value >= min && value <= max) return value;
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
			Console.Write($"{prompt} (type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;
			if (!string.IsNullOrWhiteSpace(trimmed) || allowEmpty) return trimmed;

			ShowError(ValidationMessages.EmptyInput);
		}
	}


	public static DateOnly? ReadDateOnly(string prompt)
	{
		while (true)
		{
			Console.Write($"{prompt} (YYYY-MM-DD, or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;
			if (DateOnly.TryParse(input, out var date)) return date;

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

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;
			if (string.IsNullOrWhiteSpace(trimmed))
			{
				ShowError(ValidationMessages.EmptyInput);
				continue;
			}

			var validationResult = Validator.ISBNValidator(trimmed);
			if (validationResult.IsValid) return trimmed;

			ShowError(validationResult.ErrorMessage ?? ValidationMessages.InvalidISBN);
		}
	}


	public static Genre? ReadGenre(string prompt)
	{
		DisplayGenres();
		var genreId = ConsoleHelper.ReadInt(prompt, 1, Enum.GetValues<Genre>().Length);
		if (genreId is null) return null;
		return (Genre)(genreId.Value - 1);
	}


	public static bool? ReadYesNo(string prompt)
	{
		while (true)
		{
			Console.Write($"{prompt} (y/n, or type 'cancel' to abort): ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim().ToLower();
			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;

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


	private static string ReadPassword(string prompt)
	{
		Console.Write($"{prompt}: ");
		var password = new System.Text.StringBuilder();
		while (true)
		{
			var key = Console.ReadKey(intercept: true);
			if (key.Key == ConsoleKey.Enter) break;
			if (key.Key == ConsoleKey.Backspace && password.Length > 0)
			{
				password.Remove(password.Length - 1, 1);
				Console.Write("\b \b");
			}
			else if (key.Key != ConsoleKey.Backspace)
			{
				password.Append(key.KeyChar);
				Console.Write("*");
			}
		}

		Console.WriteLine();
		return password.ToString();
	}


	public static List<int>? ReadMultiSelect<T>(string prompt, IReadOnlyList<T>? items, Func<T, int> idSelector,
		Func<T, string> displayNameSelector, bool allowMultiple = true, bool allowEmpty = false)
	{
		if (items == null || items.Count == 0)
		{
			if (allowEmpty)
			{
				ShowWarning("No items available. Continuing with no selection.");
				return [];
			}

			ShowWarning("No items available to select.");
			return null;
		}

		var options = items.Select(item => new OptionItem
			{ Id = idSelector(item), DisplayName = displayNameSelector(item) }).ToList();

		while (true)
		{
			Console.WriteLine("\nAvailable options: ");
			foreach (var opt in options) Console.WriteLine($"	{opt.Id}. {opt.DisplayName}");

			var instruction = allowMultiple
				? "Enter numbers separated by commas (e.g., 1,2,3 or type 'cancel' to abort)"
				: "Enter a single number (type 'cancel' to abort)";

			Console.Write($"{prompt} .{instruction}: ");
			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim();


			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;

			if (string.IsNullOrWhiteSpace(trimmed))
			{
				if (allowEmpty) return [];

				ShowError(ValidationMessages.EmptyInput);
				continue;
			}

			var (isValid, selectedIds, error) = ParseMultiSelect(trimmed, options, allowMultiple);
			if (!isValid)
			{
				ShowWarning(error!);
				continue;
			}

			var distinctIds = selectedIds!.Distinct().ToList();
			if (distinctIds.Count != selectedIds!.Count) ShowWarning(ValidationMessages.DuplicateRemoved);
			return distinctIds;
		}
	}


	private static (bool isValid, List<int>? SelectedIds, string? error) ParseMultiSelect(string input,
		List<OptionItem> validOptions, bool allowMultiple)
	{
		var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
		var result = new List<int>();

		if (!allowMultiple && parts.Length > 1) return (false, null, ValidationMessages.NotAllowedMultiSelections);

		foreach (var part in parts)
		{
			var cleaned = part.Trim();
			if (!int.TryParse(cleaned, out var id)) return (false, null, $"{cleaned} is not a valid number.");
			if (validOptions.All(o => o.Id != id))
				return (false, null, $"ID '{id}' is not in the list. Please choose from the options above.");
			result.Add(id);
		}

		if (result.Count == 0) return (false, null, ValidationMessages.InvalidOptionSelection);
		return (true, result, null);
	}


	public static List<int>? ReadAuthors(string prompt, IReadOnlyList<AuthorDto> authors, bool allowMultiple = true,
		bool allowEmpty = false)
	{
		return ReadMultiSelect(prompt, authors, idSelector: a => a.Id,
			displayNameSelector: a => $"{a.FirstName} {a.LastName}", allowMultiple: allowMultiple,
			allowEmpty: allowEmpty);
	}


	public static List<int>? ReadTranslators(string prompt, IReadOnlyList<TranslatorDto> translators,
		bool allowMultiple = true, bool allowEmpty = true)
	{
		return ReadMultiSelect(prompt, translators, idSelector: t => t.Id,
			displayNameSelector: t => $"{t.FirstName} {t.LastName}", allowMultiple, allowEmpty);
	}


	public static List<int>? ReadRoles(string prompt, IReadOnlyList<Role> roles, bool allowMultiple = true,
		bool allowEmpty = false)
	{
		return ReadMultiSelect(prompt, roles, idSelector: r => r.Id, displayNameSelector: r => r.Name.ToString(),
			allowMultiple, allowEmpty);
	}



	private static string? GetValidString(string prompt, Func<string, ValidationResult> validator)
	{
		while (true)
		{
			var input = ReadString(prompt);

			if (input is null) return null;

			var validationResult = validator(input);
			if (validationResult.IsValid) return input;

			ShowError(validationResult.ErrorMessage ?? ValidationMessages.InvalidInput);
		}
	}


	public static string? GetValidName(string prompt, int minLength, int maxLength)
	{
		return GetValidString(prompt, input => Validator.NameValidator(input, minLength, maxLength));
	}


	public static string? GetValidNationalCode(string prompt)
	{
		return GetValidString(prompt, Validator.NationalCodeValidator);
	}


	public static string? GetValidEmail(string prompt) { return GetValidString(prompt, Validator.EmailValidator); }


	public static string? GetValidPhoneNumber(string prompt)
	{
		return GetValidString(prompt, Validator.PhoneNumberValidator);
	}


	public static DateOnly? GetValidDateOnly(string prompt, Func<DateOnly, ValidationResult> validator)
	{
		while (true)
		{
			var date = ReadDateOnly(prompt);
			if (date is null) return null;

			var validationResult = validator(date.Value);
			if (validationResult.IsValid) return date;

			ShowError(validationResult.ErrorMessage ?? ValidationMessages.InvalidDate);
		}
	}


	public static DateOnly? GetValidBirthDate(string prompt)
	{
		return GetValidDateOnly(prompt, Validator.BirthDateValidator);
	}


	public static DateOnly? GetValidDate(string prompt)
	{
		return GetValidDateOnly(prompt, d => Validator.DateValidator(d));
	}


	public static string GetValidPassword(string prompt)
	{
		while (true)
		{
			var input = ReadPassword(prompt);
			var validationResult = Validator.PasswordValidator(input);
			if (validationResult.IsValid) return input;

			ShowError(validationResult.ErrorMessage ?? ValidationMessages.InvalidInput);
		}
	}


	public static void ShowResult<T>(ServiceResult<T> result) where T : class
	{
		if (result.Success)
		{
			if (result.Status == ResultStatus.Warning)
				ShowWarning(result.Message ?? "Warning.");
			else
				ShowSuccess(result.Message ?? "Operation completed successfully.");
		}
		else
		{
			ShowError(result.Message ?? "Operation failed.");
		}
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


	public static void DisplayGenres()
	{
		Console.WriteLine("\n{0,3} {1, 15}", "ID", "Genre Name");
		Console.WriteLine("============================");
		var values = Enum.GetValues<Genre>();
		for (var i = 0; i < values.Length; i++)
		{
			Console.WriteLine("{0,3} {1, 15}", i + 1, values.GetValue(i));
		}

		Console.WriteLine("============================");
	}


	public static LoanStatus? ReadLoanStatus(string prompt)
	{
		Console.WriteLine("1. Borrowed");
		Console.WriteLine("2. Returned");
		Console.WriteLine("3. Overdue");
		Console.WriteLine("4. Lost");
		var choice = ReadInt(prompt, 1, 4);
		if (choice is null) return null;

		return (LoanStatus)(choice.Value - 1);
	}

	public static void Pause() 
	{
		ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}
}