namespace LibraryManagementSystem.Helpers;

// This static class loop until valid input is received.
public static class ConsoleHelper
{
	public static int? ReadInt(string prompt, int min, int max)
	{
		while (true)
		{
			Console.Write($"{prompt} (or type 'cancel' to abort): ");
			string input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
				return null;

			if (int.TryParse(input, out int value))
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
			string input = Console.ReadLine() ?? string.Empty;
			string trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(trimmed) && !allowEmpty)
			{
				Console.WriteLine("Input cannot be empty. Please try again.");
				continue;
			}

			return trimmed;
		}
	}

	public static DateOnly? ReadDateOnly(string prompt)
	{
		while (true)
		{
			Console.Write($"{prompt} (YYYY-MM-DD, or type 'cancel' to abort): ");
			string input = Console.ReadLine() ?? string.Empty;

			if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
				return null;

			if (DateOnly.TryParse(input, out DateOnly date))
				return date;
			Console.WriteLine("Invalid date format. Please use YYYY-MM-DD (e.g., 2026-12-27).");
		}
	}

	public static string? ReadISBN(string prompt, bool allowEmpty = false)
	{
		while (true)
		{
			Console.Write($"{prompt} ('978-92-95055-02-5', or type 'cancel' to abort): ");
			string input = Console.ReadLine() ?? string.Empty;
			string trimmed = input.Trim();

			if (trimmed.Equals("cancel", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(trimmed) && !allowEmpty)
			{
				Console.WriteLine("Input cannot be empty. Please try again.");
				continue;
			}
			return trimmed;
		}
	}
}