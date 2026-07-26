using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class AuthorPrinter
{
	public static void PrintDetails(AuthorDto author)
	{
		Console.WriteLine("\nAuthor Details:");
		Console.WriteLine("{0, -20} [{1}]", "Name:", author.FullName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", author.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", author.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", author.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", author.BirthDate);
		Console.WriteLine("{0, -20} [{1}]", "Biography:", author.Biography);
		Console.WriteLine("{0, -20} [{1} {2}]", "Books:", author.BookCount, "associated books");
	}


	public static void PrintTable(IReadOnlyList<AuthorDto> authors)
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
			return;
		}

		Console.WriteLine("{0,-3} {1, -30} {2, -40}", "ID", "Author Name", "Email Address");
		Console.WriteLine(new string('=', 90));

		foreach (var author in authors)
			Console.WriteLine("{0,-3} {1, -30} {2, -40}", author.Id, author.FullName, author.Email);
		Console.WriteLine(new string('=', 90));
	}
}