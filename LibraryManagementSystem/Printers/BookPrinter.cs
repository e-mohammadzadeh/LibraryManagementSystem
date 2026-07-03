using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Printers;

public static class BookPrinter
{
	public static void PrintDetails(Book book)
	{
		Console.WriteLine("\nBook Details:");

		Console.WriteLine("{0, -30} [{1}]", "Name:", book.BookName);
		Console.WriteLine("{0, -30} [{1}]", "ISBN:", book.InternationalStandardBookNumber);
		Console.WriteLine("{0, -30} [{1}]", "Author:", $"{book.Author.FirstName} {book.Author.LastName}");
		Console.WriteLine("{0, -30} [{1}]", "Author's Email:", book.Author.Email);
		Console.WriteLine("{0, -30} [{1}]", "Publication Year:", book.PublishDate);
		Console.WriteLine("{0, -30} [{1}]", "Genre:", book.Genre);
		Console.WriteLine("{0, -30} [{1}]", "Total Copies:", book.TotalCopies);
		Console.WriteLine("{0, -30} [{1}]", "Available Copies:", book.AvailableCopies);
		Console.WriteLine("{0, -30} [{1}]", "Description:", book.Description);
	}


	public static void PrintTable(IReadOnlyList<Book> books)
	{
		if (books.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		Console.WriteLine("{0,3} {1, 30} {2, 30} {3, 30}", "ID", "Book Name", "Author Name", "ISBN");
		Console.WriteLine(
			"===========================================================================================================");

		foreach (var book in books)
		{
			var authorName = book.Author.FirstName + " " + book.Author.LastName;
			Console.WriteLine("{0,3} {1, 30} {2, 30} {3, 30}", book.BookId, book.BookName, authorName,
				book.InternationalStandardBookNumber);
		}

		Console.WriteLine(
			"===========================================================================================================");
	}


	//BookPrinter.PrintSummary(book);

	//BookPrinter.PrintDetails(book);

	//BookPrinter.PrintTable(books);
}