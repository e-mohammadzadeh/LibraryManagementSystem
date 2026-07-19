using LibraryManagementSystem.Application.Common;
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
			Console.Write($"{prompt} (or type 'cancel' to abort): ");
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
			Console.Write($"{prompt} (or type 'cancel' to abort): ");
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


	public static List<int>? ReadRoles(string prompt, IReadOnlyList<Role> availableRoles, bool allowMultiple = true)
	{
		// Build the menu dynamically from the Enum
		var roleOptions = GetRoleOptions(availableRoles);
		string instructions = allowMultiple
			? "(enter numbers separated by commas, e.g., 1,2. Or type 'cancel' to abort)"
			: "(enter a single number. Or type 'cancel' to abort)";

		while (true)
		{
			DisplayRoleMenu(roleOptions);
			Console.Write($"{prompt} {instructions}: ");

			var input = Console.ReadLine() ?? string.Empty;
			var trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase)) return null;
			if (string.IsNullOrEmpty(trimmed))
			{
				ShowError(ValidationMessages.InvalidRoleSelection);
				continue;
			}

			// Parse and validate the input
			var (isValid, roleIds, errorMessage) = ParseRoleInput(trimmed, roleOptions, allowMultiple);
			if (!isValid)
			{
				ShowError(errorMessage!);
				continue;
			}

			// Remove duplicates 
			var distinctRoles = roleIds?.Distinct().ToList();
			if (allowMultiple && distinctRoles?.Count != roleIds?.Count)
			{
				ShowWarning(ValidationMessages.DuplicateRolesRemoved);
			}

			return distinctRoles;
		}
	}


	private static List<RoleOption> GetRoleOptions(IReadOnlyList<Role> availableRoles)
	{
		return [.. availableRoles.Select(role => new RoleOption { Id = role.Id, Name = role.Name.ToString() })];
	}


	private static void DisplayRoleMenu(List<RoleOption> options)
	{
		Console.WriteLine("\nAvailable Roles:");
		foreach (var option in options)
		{
			Console.WriteLine($"  {option.Id}. {option.Name}");
		}
	}


	private static (bool IsValid, List<int>? RoleIds, string? ErrorMessage) ParseRoleInput(string trimmedInput,
		List<RoleOption> validOptions, bool allowMultiple)
	{
		var validIds = validOptions.Select(o => o.Id).ToList();
		var parts = trimmedInput.Split(',', StringSplitOptions.RemoveEmptyEntries);

		if (!allowMultiple && parts.Length > 1)
		{
			return (false, null, ValidationMessages.NotAllowedMultipleRole);
		}

		var result = new List<int>();
		foreach (var part in parts)
		{
			var cleaned = part.Trim();
			if (string.IsNullOrEmpty(cleaned))
				continue;

			if (!int.TryParse(cleaned, out var id))
			{
				return (false, null, $"'{cleaned}' is not a valid number.");
			}

			if (!validIds.Contains(id))
			{
				return (false, null, $"'{id}' is not a valid role. Available IDs: {string.Join(", ", validIds)}.");
			}

			result.Add(id);
		}

		if (result.Count == 0)
		{
			return (false, null, "No valid roles were found. Please enter at least one role.");
		}

		if (!allowMultiple && result.Count > 1)
		{
			return (false, null, ValidationMessages.NotAllowedMultipleRole);
		}

		return (true, result, null);
	}


	// Simple DTO for the menu
	private class RoleOption
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
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


	public static string? GetValidEmail(string prompt)
	{
		return GetValidString(prompt, Validator.EmailValidator);
	}


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
}