using System.Text;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class TranslatorPrinter
{
	public static void PrintDetails(TranslatorDto translator)
	{
		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

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


	public static void PrintTable(IReadOnlyList<TranslatorDto> translators)
	{
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.Clear();
		Console.WriteLine("\n{0,-3} {1, -30} {2, -40}", "ID", "Translator Name", "Email Address");
		Console.WriteLine(new string('=', 90));


		foreach (var translator in translators)
		{
			var fullName = translator.FirstName + " " + translator.LastName;
			Console.WriteLine("{0,-3} {1, -30} {2, -40}", translator.Id, fullName, translator.Email);
		}

		Console.WriteLine(new string('=', 90));
	}
}