using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(BookDto book)
	{
		
		var authorEmails = book.BookAuthors.Select(ba => $"{ba.Author.Email}").ToList();
		var translatorEmails = book.BookTranslators.Select(ta => $"{ta.Translator.Email}").ToList();
		var authorsNameDisplay = string.Join(", ", book.Authors);
		var translatorsNameDisplay = string.Join(", ", book.Translators);
		var authorsEmailDisplay = string.Join(", ", authorEmails);
		var translatorsEmailDisplay = string.Join(", ", translatorEmails);
		Console.WriteLine("\nBook Details:");

		Console.WriteLine("{0, -30} [{1}]", "Name:", book.BookName);
		Console.WriteLine("{0, -30} [{1}]", "ISBN:", book.ISBN);
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


	public static void PrintTable(IReadOnlyList<BookDto> books)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -30} {4, -30} {5, -6}", "ID", "Book Name", "Author Name",
			"ISBN", "Translator Name", "Copies");
		Console.WriteLine(new string('=', 190));

		foreach (var book in books)
		{
			
			var authorsDisplay = book.Authors.Length > 47 ? book.Authors[..45] + "..." : book.Authors;
			var translatorsDisplay = book.Translators.Length > 27 ? book.Translators[..25] + "..." : book.Translators;

			Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -30} {4, -30} {5, -6}", book.BookId, book.BookName,
				authorsDisplay, translatorsDisplay, book.ISBN,
				$"{book.AvailableCopies}/{book.TotalCopies}");
		}

		Console.WriteLine(new string('=', 190));
	}
}