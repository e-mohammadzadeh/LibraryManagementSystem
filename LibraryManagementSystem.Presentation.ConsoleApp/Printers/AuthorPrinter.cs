using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class AuthorPrinter
{
	public static void PrintDetails(ContributorDto author)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Name", [$"{author.FirstName} {author.LastName}"]),
			("National Code", [author.NationalCode]),
			("Email", [author.Email]),
			("Phone Number", [author.PhoneNumber]),
			("Birth Date", [author.BirthDate.ToString("yyyy-MM-dd")]),
			("Biography", [author.Biography!]),
			("Created At", [author.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [author.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Author Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<ContributorDto> authors, string title = "Author List")
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return;
		}

		Console.Clear();
		var headers = new[] { "#", "Author Name", "Email Address", "Biography", "Books (ISBN)" };

		var rows = authors.Select((author, index) => (string[][])
		[
			[(index + 1).ToString()], // friendly counter
			[$"{author.FirstName} {author.LastName}"],
			[author.Email],
			[author.Biography ?? "N/A"],
			["—"] // later you can put ISBNs here
		]).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintFullTable(IReadOnlyList<ContributorDto> authors, string title = "Author Full Information")
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return;
		}

		Console.Clear();
		var headers = new[]
		{
			"#", "Author Name", "National Code", "Email Address", "Phone Number", "Birth Date", "Biography",
			"Books (ISBN)"
		};

		var rows = authors.Select((author, index) => (string[][])
		[
			[(index + 1).ToString()],
			[$"{author.FirstName} {author.LastName}"],
			[author.NationalCode],
			[author.Email],
			[author.PhoneNumber],
			[author.BirthDate.ToString("yyyy-MM-dd")],
			[author.Biography ?? "N/A"],
			["—"] // Books column – fill later
		]).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}