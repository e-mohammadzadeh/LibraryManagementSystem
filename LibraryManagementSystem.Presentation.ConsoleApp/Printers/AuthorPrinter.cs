using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class AuthorPrinter
{
	public static void PrintDetails(Author author)
	{
		Console.WriteLine("\nAuthor Details:");

		Console.WriteLine("{0, -20} [{1} {2}]", "Name:", author.FirstName, author.LastName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", author.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", author.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", author.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", author.BirthDate);
		Console.WriteLine("{0, -20} [{1}]", "Biography:", author.Biography);
		Console.WriteLine("{0, -20} [{1} {2}]", "Books:", author.Books.Count, "associated books");
	}


	public static void PrintTable(IReadOnlyList<Author> authors)
	{
		if (authors.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
			return;
		}

		Console.WriteLine("{0,-3} {1, -30} {2, -40}", "ID", "Author Name", "Email Address");
		Console.WriteLine(new string('=', 90));


		foreach (var author in authors)
		{
			var fullName = author.FirstName + " " + author.LastName;
			Console.WriteLine("{0,-3} {1, -30} {2, -40}", author.Id, fullName, author.Email);
		}

		Console.WriteLine(new string('=', 90));
	}
}