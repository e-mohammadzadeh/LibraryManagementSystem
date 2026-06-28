using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Helpers;

public static class MenuHelper
{
	public static Author? SelectAuthor(IReadOnlyList<Author> authorsList)
	{
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
			return null;
		}

		while (true)
		{
			DisplayAuthors(authorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredAuthorId = ConsoleHelper.ReadInt("Enter the number of the author you wish", 1,
				authorsList.Last().AuthorId);

			if (desiredAuthorId is null)
				return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.AuthorId == desiredAuthorId.Value);

			if (desiredAuthor != null)
				return desiredAuthor;

			ConsoleHelper.ShowError("Author not found. Please try again.");
		}
	}


	public static Book? SelectBook(IReadOnlyList<Book> booksList)
	{
		if (booksList.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return null;
		}

		while (true)
		{
			DisplayBooks(booksList);
			var desiredBookId = ConsoleHelper.ReadInt("Enter the number of the book you wish", 1,
				booksList.Last().BookId);

			if (desiredBookId is null)
				return null;

			var desiredBook = booksList.FirstOrDefault(b => b.BookId == desiredBookId.Value);
			if (desiredBook != null)
				return desiredBook;

			ConsoleHelper.ShowError("Book not found. Please try again.");
		}
	}


	public static void DisplayAuthors(IReadOnlyList<Author> authors)
	{
		if (authors.Count == 0)
			return;

		Console.WriteLine("{0,3} {1, 20} {2, 25}", "ID", "Author Name", "Email Address");
		Console.WriteLine("========================================================");

		foreach (var author in authors)
		{
			var fullName = author.FirstName + " " + author.LastName;
			Console.WriteLine("{0,3} {1, 20} {2, 25}", author.AuthorId, fullName, author.Email);
		}

		Console.WriteLine("========================================================");
	}


	public static void DisplayBooks(IReadOnlyList<Book> books)
	{
		if (books.Count == 0)
			return;

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
}