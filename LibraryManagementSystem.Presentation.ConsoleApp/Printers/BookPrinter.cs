using System.Text;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(BookDto book)
	{
		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		// One name/email per line when there are many (wraps cleanly in the value column)
		var authorNames = book.Authors.Count == 0 ? ["—"] : book.Authors.Select(a => a.FullName).ToArray();
		var authorEmails = book.Authors.Count == 0 ? ["—"] : book.Authors.Select(a => a.Email).ToArray();
		var translatorNames = book.Translators.Count == 0 ? ["—"] : book.Translators.Select(t => t.FullName).ToArray();
		var translatorEmails = book.Translators.Count == 0 ? ["—"] : book.Translators.Select(t => t.Email).ToArray();

		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Name", [book.BookName]),
			("ISBN", [book.ISBN]),
			("Author(s)", authorNames),
			("Author Email(s)", authorEmails),
			("Translator(s)", translatorNames),
			("Translator Email(s)", translatorEmails),
			("Publication Date", [book.PublishDate.ToString("yyyy-MM-dd")]),
			("Genre", [book.Genre]),
			("Publisher", [book.Publisher]),
			("Total Copies", [book.TotalCopies.ToString()]),
			("Available Copies", [book.AvailableCopies.ToString()]),
			("Description", string.IsNullOrWhiteSpace(book.Description) ? ["—"] : book.Description.Split('\n')),
			("Created At", [book.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [book.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Book Details", rows, labelWidth: 22, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<BookDto> books)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return;
		}

		Console.Clear();
		Console.WriteLine("\n{0,-3} {1, -60} {2, -50} {3, -20} {4, -30} {5, -6}", "ID", "Book Name", "Author Name",
			"ISBN", "Translator Name", "Copies");
		Console.WriteLine(new string('=', 190));

		foreach (var book in books)
		{
			var authorsDisplay = string.Join(", ", book.Authors.Select(a => a.FullName));
			authorsDisplay = authorsDisplay.Length > 47 ? authorsDisplay[..45] + "..." : authorsDisplay;
			var translatorsDisplay = string.Join(", ", book.Translators.Select(t => t.FullName));
			translatorsDisplay = translatorsDisplay.Length > 27 ? translatorsDisplay[..25] + "..." : translatorsDisplay;

			Console.WriteLine("{0,-3} {1, -60} {2, -50} {3, -20} {4, -30} {5, -6}", book.BookId, book.BookName,
				authorsDisplay, book.ISBN, translatorsDisplay, $"{book.AvailableCopies}/{book.TotalCopies}");
		}

		Console.WriteLine(new string('=', 190));
	}


	public static void PrintLoanHistory(IReadOnlyList<LoanDto> loans)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowInfo(Messages.NoLoanHistoryForBook);
			return;
		}

		Console.WriteLine($"\nLoan History ({loans.Count} loans):");
		LoanPrinter.PrintTable(loans);
	}
}