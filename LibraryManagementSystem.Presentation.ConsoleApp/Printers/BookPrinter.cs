using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;

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


	public static void PrintTable(IReadOnlyList<BookDto> books, IAuthorizationService? authorization = null)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		var showExactCopies = authorization is null
		                      || authorization.HasPermission(Permission.ViewBookDetails)
		                      || authorization.HasPermission(Permission.AddBook)
		                      || authorization.HasPermission(Permission.EditBook);

		var headers = new[]
		{
			"ID",
			"Book Name",
			"ISBN",
			"Author(s)",
			"Translator(s)",
			"Genre",
			"Publish Date",
			"Description",
			"Availability"
		};

		var rows = books.Select(book =>
		{
			var authors = book.Authors.Count > 0
				? book.Authors.Select(a => a.FullName).ToArray()
				: ["—"];

			var translators = book.Translators.Count > 0
				? book.Translators.Select(t => t.FullName).ToArray()
				: ["—"];

			var description = string.IsNullOrWhiteSpace(book.Description)
				? ["—"]
				: ConsoleTable.WrapText(book.Description, ValidationConstants.DescriptionWrapWidthInTable);

			var availability = showExactCopies
				? new[] { $"{book.AvailableCopies}/{book.TotalCopies}" }
				: new[] { book.AvailableCopies > 0 ? "Available" : "Not available" };

			var bookName = ConsoleTable.WrapText(book.BookName, ValidationConstants.BookNameWrapWidthInTable);

			return new[]
			{
				[book.BookId.ToString()],
				bookName,
				[book.ISBN],
				authors,
				translators,
				[book.Genre.ToString()],
				[book.PublishDate.ToString("yyyy-MM-dd")],
				description,
				availability
			};
		}).ToList();

		ConsoleTable.PrintTable("Book List", headers, rows);
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