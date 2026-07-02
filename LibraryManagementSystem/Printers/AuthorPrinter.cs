using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Printers;

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

		Console.WriteLine("{0,3} {1, 20} {2, 25}", "ID", "Author Name", "Email Address");
		Console.WriteLine("========================================================");

		foreach (var author in authors)
		{
			var fullName = author.FirstName + " " + author.LastName;
			Console.WriteLine("{0,3} {1, 20} {2, 25}", author.Id, fullName, author.Email);
		}

		Console.WriteLine("========================================================");
	}
}