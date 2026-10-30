using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.DTOs.Library;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
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
				$"Logged in as: {currentUser.FullName} ({currentUser.Email} - {currentUser.RoleName})");
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
		Console.WriteLine($"Logged in as: {currentUser.FullName} ({currentUser.RoleName})");
		Console.ResetColor();
		Console.WriteLine();
	}


	public static ContributorDto? SelectAuthor(IReadOnlyList<ContributorDto> authorsList,
		IAuthorizationService authorization)
	{
		if (authorsList.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
			return null;
		}

		Action<IReadOnlyList<ContributorDto>> printer =
			authorization.HasPermission(Permission.ViewContributorDetails)
				? author => AuthorPrinter.PrintFullTable(author)
				: author => AuthorPrinter.PrintTable(author);
		while (true)
		{
			printer(authorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredAuthorId =
				ConsoleHelper.ReadInt("Enter the number of the author you wish", 1, authorsList.Count);
			return desiredAuthorId is null ? null : authorsList[desiredAuthorId.Value - 1];
		}
	}



	public static ContributorDto? SelectTranslator(IReadOnlyList<ContributorDto> translatorsList,
		IAuthorizationService authorization)
	{
		if (translatorsList.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableTranslator);
			return null;
		}

		Action<IReadOnlyList<ContributorDto>> printer =
			authorization.HasAnyPermission(Permission.ViewContributorDetails, Permission.ViewAllContributors)
				? translator => TranslatorPrinter.PrintFullTable(translator)
				: translator => TranslatorPrinter.PrintTable(translator);

		while (true)
		{
			printer(translatorsList);
			// TODO	Max parameter has some logical issues when authors are removed and new authors are added.
			var desiredTranslatorId =
				ConsoleHelper.ReadInt(Messages.SelectTranslatorQuestion, 1, translatorsList.Count);
			return desiredTranslatorId is null ? null : translatorsList[desiredTranslatorId.Value - 1];
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
			var desiredId = ConsoleHelper.ReadInt(Messages.SelectBookQuestion, 1, booksList.Count);

			return desiredId is null ? null : booksList[desiredId.Value - 1];
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
			var desiredId = ConsoleHelper.ReadInt(Messages.SelectMemberQuestion, 1, usersList.Count);

			return desiredId is null ? null : usersList[desiredId.Value - 1];
		}
	}


	public static TEntity? SelectExisting<TEntity>(IReadOnlyList<TEntity> items,
		Func<IReadOnlyList<TEntity>, TEntity?> selectFn, string emptyMessage) where TEntity : class
	{
		if (items.Count != 0) return selectFn(items);
		ConsoleHelper.ShowWarning(emptyMessage);
		return null;
	}


	public static T? DisplayMenu<T>(string title, List<(T Action, string DisplayText, bool IsAvailable)> items)
	{
		var availableItems = items.Where(item => item.IsAvailable).ToList();

		if (availableItems.Count == 0) return default;

		Console.WriteLine(new string('=', 37) + $" {title} " + new string('=', 37));

		for (var i = 0; i < availableItems.Count; i++) Console.WriteLine($"{i + 1}. {availableItems[i].DisplayText}");

		Console.WriteLine(new string('=', 82));

		var choice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, availableItems.Count);

		return choice is null ? default : availableItems[choice.Value - 1].Action;
	}


	public static LoanDto? SelectActiveLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, string purpose)
	{
		Guid userId;
		if (session.IsSelfServiceMember)
			userId = session.CurrentUser.Id;
		else
		{
			var user = SelectUser(userManagementService.GetAllUsers(),
				users => UserPrinter.PrintTable(users));
			if (user is null)
			{
				ConsoleHelper.ShowWarning(Messages.UserNotFound);
				return null;
			}

			userId = user.Id;
		}

		var loans = loanManagementService.GetLoansByUser(userId, session, LoanFilter.Active);
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UserHasNoBorrowedBooks);
			return null;
		}

		Console.Clear();
		LoanPrinter.PrintTable(loans);
		var selection = ConsoleHelper.ReadInt($"Enter loan id to {purpose}", 1, loans.Count);
		return selection is null ? null : loans[selection.Value - 1];
	}
}