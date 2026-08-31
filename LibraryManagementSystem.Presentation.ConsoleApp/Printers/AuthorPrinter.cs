using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class AuthorPrinter
{
	public static void PrintDetails(AuthorDto author)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Name", [author.FullName]),
			("National Code", [author.NationalCode]),
			("Email", [author.Email]),
			("Phone Number", [author.PhoneNumber]),
			("Birth Date", [author.BirthDate.ToString("yyyy-MM-dd")]),
			("Biography", [author.Biography!]),
			("Books", [$"{author.BookCount} associated books"]),
			("Created At", [author.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [author.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Author Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<AuthorDto> authors, string title= "Author List")
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return;
		}

		Console.Clear();
		var headers = new[] { "ID", "Author Name", "Email Address", "Biography", "Books (ISBN)" };

		var rows = authors.Select(author => new[]
		{
			[author.Id.ToString()],
			[author.FullName],
			[author.Email],
			[author.Biography ?? "N/A"],
			author.Books.Count > 0 ? author.Books.Select(b => $"{b.BookName} ({b.ISBN})").ToArray() : ["No books"]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintFullTable(IReadOnlyList<AuthorDto> authors, string title= "Author Full Information")
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return;
		}

		Console.Clear();
		var headers = new[]
		{
			"ID", "Author Name", "National Code", "Email Address", "Phone Number", "Birth Date", "Biography",
			"Books (ISBN)"
		};

		var rows = authors.Select(author => new[]
		{
			[author.Id.ToString()],
			[author.FullName],
			[author.NationalCode],
			[author.Email],
			[author.PhoneNumber],
			[author.BirthDate.ToString("yyyy-MM-dd")],
			[author.Biography ?? "N/A"],
			author.Books.Count > 0
				? author.Books.Select(book => $"{book.BookName} ({book.ISBN})").ToArray()
				: ["No books"]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}