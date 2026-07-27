using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(BookDto book)
	{
		var authorsNameDisplay = string.Join(", ", book.Authors.Select(a => a.FullName));
		var translatorsNameDisplay = string.Join(", ", book.Translators.Select(t => t.FullName));
		var authorsEmailDisplay = string.Join(", ", book.Authors.Select(a => a.Email));
		var translatorsEmailDisplay = string.Join(", ", book.Translators.Select(t => t.Email));
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
		Console.WriteLine("{0, -30} [{1}]", "Created At:", book.CreatedAt);
		Console.WriteLine("{0, -30} [{1}]", "Updated At:", book.UpdatedAt);
	}


	public static void PrintTable(IReadOnlyList<BookDto> books)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -15} {4, -30} {5, -6}", "ID", "Book Name", "Author Name",
			"ISBN", "Translator Name", "Copies");
		Console.WriteLine(new string('=', 190));

		foreach (var book in books)
		{
			var authorsDisplay = string.Join(", ", book.Authors.Select(a => a.FullName));
			authorsDisplay = authorsDisplay.Length > 47 ? authorsDisplay[..45] + "..." : authorsDisplay;
			var translatorsDisplay = string.Join(", ", book.Translators.Select(t => t.FullName));
			translatorsDisplay = translatorsDisplay.Length > 27 ? translatorsDisplay[..25] + "..." : translatorsDisplay;

			Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -15} {4, -30} {5, -6}", book.BookId, book.BookName,
				authorsDisplay, book.ISBN, translatorsDisplay, $"{book.AvailableCopies}/{book.TotalCopies}");
		}

		Console.WriteLine(new string('=', 190));
	}
}