using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class BookPrinter
{
	public static void PrintDetails(BookDto details)
	{
		ConsoleHelper.ClearConsole();

		// One name/email per line when there are many (wraps cleanly in the value column)
		var authorNames = details.Authors.Count == 0
			? ["—"]
			: details.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToArray();

		var authorEmails = details.Authors.Count == 0
			? ["—"]
			: details.Authors.Select(a => a.Email.ToString()).ToArray();

		var translatorNames = details.Translators.Count == 0
			? ["—"]
			: details.Translators.Select(t => $"{t.FirstName} {t.LastName}").ToArray();

		var translatorEmails = details.Translators.Count == 0
			? ["—"]
			: details.Translators.Select(t => t.Email.ToString()).ToArray();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Name", [details.Title]),
			("ISBN", [details.ISBN]),
			("Author(s)", authorNames),
			("Author Email(s)", authorEmails),
			("Translator(s)", translatorNames),
			("Translator Email(s)", translatorEmails),
			("Publication Date", [details.PublishDate.ToString("yyyy-MM-dd")]),
			("Genre", [details.Genre.ToString()]),
			("Publisher", [details.Publisher]),
			("Total Copies", [details.TotalCopies.ToString()]),
			("Available Copies", [details.AvailableCopies.ToString()]),
			("Description", string.IsNullOrWhiteSpace(details.Description) ? ["—"] : details.Description.Split('\n')),
			("Created At", [details.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [details.UpdatedAt.ToString("yyyy-MM-dd HH:mm")]),
		};

		ConsoleTable.PrintKeyValueTable("Book Details", rows, labelWidth: 22, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<BookDto> details, IAuthorizationService? authorization = null,
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
			var authors = item.Authors.Count > 0
				? item.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToArray()
				: ["—"];

			var translators = item.Translators.Count > 0
				? item.Translators.Select(t => $"{t.FirstName} {t.LastName}").ToArray()
				: ["—"];

			var description = string.IsNullOrWhiteSpace(item.Description)
				? ["—"]
				: ConsoleTable.WrapText(item.Description, ValidationConstants.DescriptionWrapWidthInTable);

			var publisher = string.IsNullOrWhiteSpace(item.Publisher)
				? ["-"]
				: ConsoleTable.WrapText(item.Publisher, ValidationConstants.DescriptionWrapWidthInTable);

			var availability = showExactCopies
				? [$"{item.AvailableCopies}/{item.TotalCopies}"]
				: new[] { item.AvailableCopies > 0 ? "Available" : "Not available" };

			var bookName = ConsoleTable.WrapText(item.Title, ValidationConstants.BookNameWrapWidthInTable);

			return new[]
			{
				[(index + 1).ToString()],
				bookName,
				[item.ISBN],
				authors,
				translators,
				[item.Genre.ToString()],
				[item.PublishDate.ToString("yyyy-MM-dd")],
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