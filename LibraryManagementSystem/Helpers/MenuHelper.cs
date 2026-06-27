using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Helpers;

public static class MenuHelper
{
	public static Author? SelectAuthor(IReadOnlyList<Author> authorsList) {
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

			if (desiredAuthorId == null)
				return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.AuthorId == desiredAuthorId.Value);

			if (desiredAuthor != null)
				return desiredAuthor;

			ConsoleHelper.ShowError("Author not found. Please try again.");
		}
	}


	public static void DisplayAuthors(IReadOnlyList<Author> authors) {
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
}