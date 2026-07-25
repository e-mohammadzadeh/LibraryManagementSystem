using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(Book book)
	{
		var authorNames = book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").ToList();
		var translatorNames = book.BookTranslators.Select(ta => $"{ta.Translator.FirstName} {ta.Translator.LastName}")
			.ToList();
		var authorEmails = book.BookAuthors.Select(ba => $"{ba.Author.Email}").ToList();
		var translatorEmails = book.BookTranslators.Select(ta => $"{ta.Translator.Email}").ToList();
		var authorsNameDisplay = string.Join(", ", authorNames);
		var translatorsNameDisplay = string.Join(", ", translatorNames);
		var authorsEmailDisplay = string.Join(", ", authorEmails);
		var translatorsEmailDisplay = string.Join(", ", translatorEmails);
		Console.WriteLine("\nBook Details:");

		Console.WriteLine("{0, -30} [{1}]", "Name:", book.BookName);
		Console.WriteLine("{0, -30} [{1}]", "ISBN:", book.InternationalStandardBookNumber);
		Console.WriteLine("{0, -30} [{1}]", "Author:", authorsNameDisplay);
		Console.WriteLine("{0, -30} [{1}]", "Author's Email:", authorsEmailDisplay);
		Console.WriteLine("{0, -30} [{1}]", "Translator:", translatorsNameDisplay);
		Console.WriteLine("{0, -30} [{1}]", "Translator's Email:", translatorsEmailDisplay);
		Console.WriteLine("{0, -30} [{1}]", "Publication Year:", book.PublishDate);
		Console.WriteLine("{0, -30} [{1}]", "Genre:", book.Genre);
		Console.WriteLine("{0, -30} [{1}]", "Publisher:", book.Publisher);
		Console.WriteLine("{0, -30} [{1}]", "Total Copies:", book.TotalCopies);
		Console.WriteLine("{0, -30} [{1}]", "Available Copies:", book.AvailableCopies);
		Console.WriteLine("{0, -30} [{1}]", "Description:", book.Description);
	}


	public static void PrintTable(IReadOnlyList<Book> books)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -30} {4, -30} {5, -6}", "ID", "Book Name", "Author Name",
			"ISBN", "Translator Name", "Copies");
		Console.WriteLine(new string('=', 170));

		foreach (var book in books)
		{
			var authorNames = string.Join(", ",
				book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}"));
			var translatorNames = string.Join(", ",
				book.BookTranslators.Select(ta => $"{ta.Translator.FirstName} {ta.Translator.LastName}"));
			var authorsDisplay = authorNames.Length > 47 ? authorNames[..45] + "..." : authorNames;
			var translatorsDisplay = translatorNames.Length > 27 ? translatorNames[..25] + "..." : translatorNames;

			Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -30} {4, -30} {5, -6}", book.BookId, book.BookName,
				authorsDisplay, translatorsDisplay, book.InternationalStandardBookNumber,
				$"{book.AvailableCopies}/{book.TotalCopies}");
		}

		Console.WriteLine(new string('=', 170));
	}
}