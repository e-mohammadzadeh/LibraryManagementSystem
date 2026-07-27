using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Application.DTOs.Users;
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


	public static AuthorDto? SelectAuthor(IReadOnlyList<AuthorDto> authorsList)
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
				authorsList.Max(a => a.Id));

			if (desiredAuthorId is null) return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.Id == desiredAuthorId.Value);
			if (desiredAuthor != null) return desiredAuthor;

			ConsoleHelper.ShowError("Author not found. Please try again.");
		}
	}



	public static TranslatorDto? SelectTranslator(IReadOnlyList<TranslatorDto> translatorsList)
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
				translatorsList.Max(t => t.Id));

			if (desiredTranslatorId is null) return null;

			var desiredTranslator = translatorsList.FirstOrDefault(t => t.Id == desiredTranslatorId.Value);
			if (desiredTranslator != null) return desiredTranslator;

			ConsoleHelper.ShowError("Translator not found. Please try again.");
		}
	}


	public static BookDto? SelectBook(IReadOnlyList<BookDto> booksList)
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
				booksList.Max(b => b.BookId));

			if (desiredBookId is null) return null;

			var desiredBook = booksList.FirstOrDefault(b => b.BookId == desiredBookId.Value);
			if (desiredBook != null) return desiredBook;

			ConsoleHelper.ShowError("Book not found. Please try again.");
		}
	}


	public static UserDto? SelectUser(IReadOnlyList<UserDto> usersList)
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
				usersList.Max(u => u.Id));

			if (desiredMemberId is null) return null;

			var desiredMember = usersList.FirstOrDefault(m => m.Id == desiredMemberId.Value);
			if (desiredMember != null) return desiredMember;

			ConsoleHelper.ShowError("Member not found. Please try again.");
		}
	}


	public static TEntity? SelectExisting<TEntity>(IReadOnlyList<TEntity> items,
		Func<IReadOnlyList<TEntity>, TEntity?> selectFn, string emptyMessage) where TEntity : class
	{
		if (items.Count != 0) return selectFn(items);
		ConsoleHelper.ShowWarning(emptyMessage);
		return null;
	}
}