using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class TranslatorPrinter
{
	public static void PrintDetails(ContributorDto translator)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			//("ID", [translator.Id.ToString()]),
			("Name", [$"{translator.FirstName} {translator.LastName}"]),
			("National Code", [translator.NationalCode]),
			("Email", [translator.Email]),
			("Phone Number", [translator.PhoneNumber]),
			("Birth Date", [translator.BirthDate.ToString("yyyy-MM-dd")]),
			("Created At", [translator.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [translator.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Translator Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<ContributorDto> translators, string title = "Translator List")
	{
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.Clear();
		var headers = new[] { "#", "Translator Name", "Email Address", "Biography", "Books (ISBN)" };

		var rows = translators.Select((translator, index) => (string[][])
		[
			[(index + 1).ToString()],
			[$"{translator.FirstName} {translator.LastName}"],
			[translator.Email],
			[translator.Biography ?? "N/A"],
			["—"]
		]).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintFullTable(IReadOnlyList<ContributorDto> translators,
		string title = "Translator Full Information")
	{
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.Clear();
		var headers = new[]
			{ "#", "Translator Name", "National Code", "Email Address", "Phone Number", "Birth Date", "Biography" };

		var rows = translators.Select((translator, index) => (string[][])
		[
			[(index + 1).ToString()],
			[$"{translator.FirstName} {translator.LastName}"],
			[translator.NationalCode],
			[translator.Email],
			[translator.PhoneNumber],
			[translator.BirthDate.ToString("yyyy-MM-dd")],
			[translator.Biography ?? "N/A"],
		]).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}