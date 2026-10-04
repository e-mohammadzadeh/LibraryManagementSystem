using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(BookDetailsDto details)
	{
		ConsoleHelper.ClearConsole();
		var book = details.Book;

		// One name/email per line when there are many (wraps cleanly in the value column)
		var authorNames = details.Authors.Count == 0
			? ["—"]
			: details.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToArray();
		var authorEmails = details.Authors.Count == 0 ? ["—"] : details.Authors.Select(a => a.Email).ToArray();
		var translatorNames = details.Translators.Count == 0
			? ["—"]
			: details.Translators.Select(t => $"{t.FirstName} {t.LastName}").ToArray();
		var translatorEmails =
			details.Translators.Count == 0 ? ["—"] : details.Translators.Select(t => t.Email).ToArray();

		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Name", [book.Title]),
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


	public static void PrintTable(IReadOnlyList<BookDetailsDto> details, IAuthorizationService? authorization = null,
		string title = "Book List")
	{
		if (details.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return;
		}

		ConsoleHelper.ClearConsole();
		var showExactCopies = authorization is null
		                      || authorization.HasPermission(Permission.ViewBookDetails)
		                      || authorization.HasPermission(Permission.AddBook)
		                      || authorization.HasPermission(Permission.EditBook);

		var headers = new[]
		{
			"#", "Book Name", "ISBN", "Author(s)", "Translator(s)", "Genre", "Publish Date", "Description",
			"Publisher",
			"Availability"
		};

		var rows = details.Select((item, index) =>
		{
			var book = item.Book;
			var authors = item.Authors.Count > 0
				? item.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToArray()
				: ["—"];

			var translators = item.Translators.Count > 0
				? item.Translators.Select(t => $"{t.FirstName} {t.LastName}").ToArray()
				: ["—"];

			var description = string.IsNullOrWhiteSpace(book.Description)
				? ["—"]
				: ConsoleTable.WrapText(book.Description, ValidationConstants.DescriptionWrapWidthInTable);

			var publisher = string.IsNullOrWhiteSpace(book.Publisher)
				? ["-"]
				: ConsoleTable.WrapText(book.Publisher, ValidationConstants.DescriptionWrapWidthInTable);

			var availability = showExactCopies
				? [$"{book.AvailableCopies}/{book.TotalCopies}"]
				: new[] { book.AvailableCopies > 0 ? "Available" : "Not available" };

			var bookName = ConsoleTable.WrapText(book.Title, ValidationConstants.BookNameWrapWidthInTable);

			return new[]
			{
				[(index + 1).ToString()],
				bookName,
				[book.ISBN],
				authors,
				translators,
				[book.Genre],
				[book.PublishDate.ToString("yyyy-MM-dd")],
				description,
				publisher,
				availability
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
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