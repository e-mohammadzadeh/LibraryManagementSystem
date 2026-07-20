using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class MenuHelper
{
	public static void Print(LibraryStatisticsDto statistics)
	{
		Console.WriteLine(new string('=', 32) + " LIBRARY DASHBOARD " + new string('=', 32));
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write($"Books: {statistics.TotalBooks}\t");
		Console.Write($"Authors: {statistics.TotalAuthors}\t");
		Console.Write($"Translators: {statistics.TotalTranslators}\t");
		Console.Write($"Users: {statistics.TotalUsers}\t");
		Console.WriteLine($"Active Loans: {statistics.TotalActiveLoans}\n");
		Console.ResetColor();
	}


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



	public static Translator? SelectTranslator(IReadOnlyList<Translator> translatorsList)
	{
		if (translatorsList.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableTranslator);
			return null;
		}

		while (true)
		{
			TranslatorPrinter.PrintTable(translatorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredTranslatorId = ConsoleHelper.ReadInt("Enter the number of the translator you wish", 1,
				translatorsList.Last().Id);

			if (desiredTranslatorId is null)
				return null;

			var desiredTranslator = translatorsList.FirstOrDefault(t => t.Id == desiredTranslatorId.Value);
			if (desiredTranslator != null)
				return desiredTranslator;

			ConsoleHelper.ShowError("Translator not found. Please try again.");
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


	public static User? SelectUser(IReadOnlyList<User> usersList)
	{
		if (usersList.Count is 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableUser);
			return null;
		}

		while (true)
		{
			UserPrinter.PrintTable(usersList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredMemberId = ConsoleHelper.ReadInt("Enter the number of the member you wish", 1,
				usersList.Last().Id);

			if (desiredMemberId is null) return null;

			var desiredMember = usersList.FirstOrDefault(m => m.Id == desiredMemberId.Value);
			if (desiredMember != null) return desiredMember;

			ConsoleHelper.ShowError("Member not found. Please try again.");
		}
	}
}