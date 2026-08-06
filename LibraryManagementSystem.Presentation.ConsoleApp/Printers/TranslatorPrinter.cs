using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class TranslatorPrinter
{
	public static void PrintDetails(TranslatorDto translator) {
		Console.WriteLine("\nTranslator Details:");
		Console.WriteLine("{0, -20} [{1}]", "ID:", translator.Id);
		Console.WriteLine("{0, -20} [{1}]", "Name:", translator.FullName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", translator.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", translator.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", translator.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", translator.BirthDate);
		Console.WriteLine("{0, -20} [{1} {2}]", "Books:", translator.BookCount, "associated books");
		Console.WriteLine("{0, -20} [{1}]", "Created At:", translator.CreatedAt);
		Console.WriteLine("{0, -20} [{1}]", "Updated At:", translator.UpdatedAt);
	}


	public static void PrintTable(IReadOnlyList<TranslatorDto> translators) {
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return;
		}

		Console.WriteLine("{0,-3} {1, -30} {2, -40}", "ID", "Translator Name", "Email Address");
		Console.WriteLine(new string('=', 90));


		foreach (var translator in translators)
		{
			var fullName = translator.FirstName + " " + translator.LastName;
			Console.WriteLine("{0,-3} {1, -30} {2, -40}", translator.Id, fullName, translator.Email);
		}

		Console.WriteLine(new string('=', 90));
	}
}