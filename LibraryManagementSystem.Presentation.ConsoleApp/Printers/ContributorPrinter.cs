using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class ContributorPrinter
{
	public static void PrintDetails(ContributorDto contributor, string? title = null)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("First Name", [contributor.FirstName]),
			("Last Name", [contributor.LastName]),
			("National Code", [contributor.NationalCode]),
			("Email", [contributor.Email]),
			("Phone Number", [contributor.PhoneNumber]),
			("Birth Date", [contributor.BirthDate.ToString("yyyy-MM-dd")]),
			("Biography", [contributor.Biography ?? "N/A"]),
			("Created At", [contributor.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [contributor.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable(title, rows, labelWidth: 18, valueWidth: 55);
	}

	
	public static void PrintTable(IReadOnlyList<ContributorDto> list, string? title = null) =>
		PrintTableCore(list, title, includeDetails: false);


	public static void PrintFullTable(IReadOnlyList<ContributorDto> list, string? title = null) =>
		PrintTableCore(list, title, includeDetails: true);


	private static void PrintTableCore(IReadOnlyList<ContributorDto> list, string? title, bool includeDetails)
	{
		if (list.Count == 0)
		{
			ConsoleHelper.ShowWarning("No contributors found.");
			return;
		}

		Console.Clear();

		string[] headers = includeDetails
			? ["#", "First Name", "Last Name", "National Code", "Email", "Phone Number", "Birth Date", "Biography", "Books (ISBN)"]
			: ["#", "First Name", "Last Name", "Email", "Biography"];

		var rows = list.Select((c, i) =>
		{
			if (includeDetails)
				return
				[
					[(i + 1).ToString()],
					[c.FirstName],
					[c.LastName],
					[c.NationalCode],
					[c.Email],
					[c.PhoneNumber],
					[c.BirthDate.ToString("yyyy-MM-dd")],
					[c.Biography ?? "N/A"],
					["—"]
				];

			return new string[][]
			{
				[(i + 1).ToString()],
				[c.FirstName],
				[c.LastName],
				[c.Email.ToString()],
				[c.Biography ?? "N/A"]
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}