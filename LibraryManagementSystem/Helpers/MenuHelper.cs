using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Printers;

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
			AuthorPrinter.PrintTable(authorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredAuthorId = ConsoleHelper.ReadInt("Enter the number of the author you wish", 1,
				authorsList.Last().Id);

			if (desiredAuthorId is null) return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.Id == desiredAuthorId.Value);
			if (desiredAuthor != null) return desiredAuthor;

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
			BookPrinter.PrintTable(booksList);
			var desiredBookId = ConsoleHelper.ReadInt("Enter the number of the book you wish", 1,
				booksList.Last().BookId);

			if (desiredBookId is null) return null;

			var desiredBook = booksList.FirstOrDefault(b => b.BookId == desiredBookId.Value);
			if (desiredBook != null) return desiredBook;

			ConsoleHelper.ShowError("Book not found. Please try again.");
		}
	}


	public static Member? SelectMember(IReadOnlyList<Member> membersList) 
	{
		if (membersList.Count is 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableMember);
			return null;
		}

		while (true)
		{
			MemberPrinter.PrintTable(membersList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredMemberId = ConsoleHelper.ReadInt("Enter the number of the member you wish", 1,
				membersList.Last().Id);

			if (desiredMemberId is null) return null;

			var desiredMember = membersList.FirstOrDefault(m => m.Id == desiredMemberId.Value);
			if (desiredMember != null) return desiredMember;

			ConsoleHelper.ShowError("Member not found. Please try again.");
		}
	}
}