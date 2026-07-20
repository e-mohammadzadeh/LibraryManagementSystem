using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class TranslatorPrinter
{
	public static void PrintDetails(Translator translator) {
		Console.WriteLine("\nTranslator Details:");

		Console.WriteLine("{0, -20} [{1} {2}]", "Name:", translator.FirstName, translator.LastName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", translator.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", translator.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", translator.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", translator.BirthDate);
		Console.WriteLine("{0, -20} [{1} {2}]", "Books:", translator.Books.Count, "associated books");
	}


	public static void PrintTable(IReadOnlyList<Translator> translators) {
		if (translators.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableTranslator);
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