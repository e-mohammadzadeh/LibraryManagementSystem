using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

public static class MenuHelper
{
	public static void Print(ServiceResult<LibraryStatisticsDto> statistics, AuthUserDto? currentUser = null)
	{
		if (currentUser is not null)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(
				$"Logged in as: {currentUser.FullName} ({currentUser.Email} - {string.Join(", ", currentUser.Roles)})");
			Console.ResetColor();
			Console.WriteLine();
		}

		if (!statistics.Success || statistics.Data is null) return;

		var s = statistics.Data;
		Console.WriteLine(new string('=', 32) + " LIBRARY DASHBOARD " + new string('=', 32));
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write($"Books: {s.TotalBooks}\t");
		Console.Write($"Authors: {s.TotalAuthors}\t");
		Console.Write($"Translators: {s.TotalTranslators}\t");
		Console.Write($"Users: {s.TotalUsers}\t");
		Console.WriteLine($"Active Loans: {s.TotalActiveLoans}\n");
		Console.ResetColor();
	}


	public static void PrintCurrentUserOnly(AuthUserDto? currentUser = null)
	{
		if (currentUser is null) return;
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine($"Logged in as: {currentUser.FullName} ({string.Join(", ", currentUser.Roles)})");
		Console.ResetColor();
		Console.WriteLine();
	}


	public static AuthorDto? SelectAuthor(IReadOnlyList<AuthorDto> authorsList, IAuthorizationService authorization)
	{
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return null;
		}

		Action<IReadOnlyList<AuthorDto>> printer =
			authorization.HasPermission(Permission.ViewAuthorFullDetails)
				? author => AuthorPrinter.PrintFullTable(author)
				: author => AuthorPrinter.PrintTable(author);
		while (true)
		{
			printer(authorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredAuthorId = ConsoleHelper.ReadInt("Enter the number of the author you wish", 1,
				authorsList.Max(a => a.Id));

			if (desiredAuthorId is null) return null;

			var desiredAuthor = authorsList.FirstOrDefault(a => a.Id == desiredAuthorId.Value);
			if (desiredAuthor != null) return desiredAuthor;

			ConsoleHelper.ShowError(Messages.AuthorNotFound);
		}
	}



	public static TranslatorDto? SelectTranslator(IReadOnlyList<TranslatorDto> translatorsList,
		IAuthorizationService authorization)
	{
		if (translatorsList.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return null;
		}

		Action<IReadOnlyList<TranslatorDto>> printer =
			authorization.HasAnyPermission(Permission.ViewTranslatorDetails, Permission.ViewAllTranslators)
				? translator => TranslatorPrinter.PrintFullTable(translator)
				: translator => TranslatorPrinter.PrintTable(translator);

		while (true)
		{
			printer(translatorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredTranslatorId =
				ConsoleHelper.ReadInt(Messages.SelectTranslatorQuestion, 1, translatorsList.Max(t => t.Id));

			if (desiredTranslatorId is null) return null;

			var desiredTranslator = translatorsList.FirstOrDefault(t => t.Id == desiredTranslatorId.Value);
			if (desiredTranslator != null) return desiredTranslator;

			ConsoleHelper.ShowError(Messages.TranslatorNotFound);
		}
	}


	public static BookDto? SelectBook(IReadOnlyList<BookDto> booksList, string title)
	{
		if (booksList.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return null;
		}

		while (true)
		{
			BookPrinter.PrintTable(booksList, authorization: null, title);
			var desiredBookId = ConsoleHelper.ReadInt(Messages.SelectBookQuestion, 1, booksList.Max(b => b.BookId));

			if (desiredBookId is null) return null;

			var desiredBook = booksList.FirstOrDefault(b => b.BookId == desiredBookId.Value);
			if (desiredBook != null) return desiredBook;

			ConsoleHelper.ShowError(Messages.BookNotFound);
		}
	}


	public static UserDto? SelectUser(IReadOnlyList<UserDto> usersList, Action<IReadOnlyList<UserDto>> printer)
	{
		if (usersList.Count is 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableUser);
			return null;
		}

		while (true)
		{
			printer(usersList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredMemberId = ConsoleHelper.ReadInt(Messages.SelectMemberQuestion, 1, usersList.Max(u => u.Id));

			if (desiredMemberId is null) return null;

			var desiredMember = usersList.FirstOrDefault(m => m.Id == desiredMemberId.Value);
			if (desiredMember != null) return desiredMember;

			ConsoleHelper.ShowError(Messages.UserNotFound);
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