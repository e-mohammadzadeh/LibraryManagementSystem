using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class TranslatorPrinter
{
	public static void PrintDetails(TranslatorDto translator)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("ID", [translator.Id.ToString()]),
			("Name", [translator.FullName]),
			("National Code", [translator.NationalCode]),
			("Email", [translator.Email]),
			("Phone Number", [translator.PhoneNumber]),
			("Birth Date", [translator.BirthDate.ToString("yyyy-MM-dd")]),
			("Books", [$"{translator.BookCount} associated books"]),
			("Created At", [translator.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [translator.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Translator Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<TranslatorDto> translators, string title = "Translator List")
	{
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.Clear();
		var headers = new[] { "ID", "Translator Name", "Email Address", "Books (ISBN)" };

		var rows = translators.Select(translator => new[]
		{
			[translator.Id.ToString()],
			[translator.FullName],
			[translator.Email],
			translator.Books.Count > 0
				? translator.Books.Select(b => $"{b.BookName} ({b.ISBN})").ToArray()
				: ["No books"]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintFullTable(IReadOnlyList<TranslatorDto> translators,
		string title = "Translator Full Information")
	{
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.Clear();
		var headers = new[]
			{ "ID", "Translator Name", "National Code", "Email Address", "Phone Number", "Birth Date", "Books (ISBN)" };

		var rows = translators.Select(translator => new[]
		{
			[translator.Id.ToString()],
			[translator.FullName],
			[translator.NationalCode],
			[translator.Email],
			[translator.PhoneNumber],
			[translator.BirthDate.ToString("yyyy-MM-dd")],
			translator.Books.Count > 0
				? translator.Books.Select(book => $"{book.BookName} ({book.ISBN})").ToArray()
				: ["No books"]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}