using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoanMenu
{
	public static void LoanMenuController(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.BorrowBook,
			    Permission.ReturnBook,
			    Permission.RenewLoan,
			    Permission.ViewActiveLoans,
			    Permission.ViewOverdueLoans,
			    Permission.ViewLoanHistory,
			    Permission.MyActiveLoans,
			    Permission.ViewActiveLoansByUser,
			    Permission.ViewActiveLoansByBook,
			    Permission.MyOverdueLoans,
			    Permission.ViewOverdueLoansByUser,
			    Permission.ViewOverdueLoansByBook,
			    Permission.LoanHistoryByUser,
			    Permission.LoanHistoryByBook,
			    Permission.FullLibraryHistory,
			    Permission.SearchLoans))
		{
			return;
		}


		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (LoanMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.BorrowBook, Messages.AccessDenied))
						break;
					Console.Clear();
					BorrowBook(loanManagementService, bookManagementService, userManagementService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ReturnBook, Messages.AccessDenied))
						break;
					Console.Clear();
					ReturnBook(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RenewLoan, Messages.AccessDenied))
						break;
					Console.Clear();
					RenewLoan(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchLoans, Messages.AccessDenied))
						break;
					Console.Clear();
					SearchLoan(loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.MyActiveLoans, Permission.ViewActiveLoans,
						    Permission.MyOverdueLoans, Permission.ViewOverdueLoans))
						break;
					Console.Clear();
					ViewLoans(loanManagementService, userManagementService, bookManagementService, session,
						authorization, statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
		}
	}



	private static int LoanMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Borrow Book", authorization.HasPermission(Permission.BorrowBook)),
			(2, "Return Book", authorization.HasPermission(Permission.ReturnBook)),
			(3, "Renew Loan", authorization.HasPermission(Permission.RenewLoan)),
			(4, "Search Loans", authorization.HasPermission(Permission.SearchLoans)),
			(5, "View Loans",
				authorization.HasAnyPermission(Permission.MyActiveLoans, Permission.ViewActiveLoans,
					Permission.MyOverdueLoans, Permission.ViewOverdueLoans)),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " LOAN MENU " + new string('=', 36));

			var displayNumber = 1;
			foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

			Console.WriteLine(new string('=', 83));
			var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
			return availableItems[choice!.Value - 1].ActionId;
		}
	}


	private static void BorrowBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, UserManagementService userManagementService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		Guid userId;
		UserDto? user;
		if (session.IsSelfServiceMember)
		{
			userId = session.CurrentUser.Id;
			user = userManagementService.GetUserById(userId);
		}
		else
		{
			user = MenuHelper.SelectUser(userManagementService.GetAllUsers(), users => UserPrinter.PrintTable(users));
			if (user is null)
			{
				ConsoleHelper.ShowWarning(Messages.UserNotFound);
				return;
			}

			userId = user.Id;
		}

		var eligibilityResult = authorization.CheckBorrowEligibility(user!);
		if (!eligibilityResult.Success)
		{
			ConsoleHelper.ShowError(eligibilityResult.Message!);
			return;
		}

		var availableBooks = bookManagementService.GetAvailableBooks(EntityFilter.Active);
		if (availableBooks.Count is 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableBookToBorrow);
			return;
		}

		BookPrinter.PrintTable(availableBooks, authorization);
		BookDto selectedBook;
		while (true)
		{
			var input = ConsoleHelper.ReadInt("Enter your desired book id to borrow", 1, availableBooks.Count);
			if (input is null) return;
			selectedBook = availableBooks[input.Value - 1];
			break;
		}

		var dto = new LoanDto { BookId = selectedBook.Id, UserId = userId };
		var result = loanManagementService.BorrowBook(dto, session);
		ConsoleHelper.ShowResult(result);
	}


	private static LoanDto? SelectActiveLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, string purpose)
	{
		Guid userId;
		if (session.IsSelfServiceMember)
			userId = session.CurrentUser.Id;
		else
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
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


	private static void ReturnBook(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "return");
		if (loan is null) return;

		var result = loanManagementService.ReturnBook(loan.Id, session);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "renew");
		if (loan is null) return;

		var result = loanManagementService.RenewLoan(loan.Id, session);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchLoan(LoanManagementService loanManagementService, ICurrentUserSession session,
		bool activeOnly = false)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING LOAN MENU " + new string('=', 36));
			var loanList = activeOnly
				? loanManagementService.GetAllLoans(session, LoanFilter.Active)
				: loanManagementService.GetAllLoans(session, LoanFilter.All);
			if (loanList.Count == 0)
			{
				ConsoleHelper.ShowWarning(activeOnly ? Messages.NotAvailableActionLoan : Messages.NotAvailableLoan);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Title");
			Console.WriteLine("{0, -20}", "2. ISBN");
			Console.WriteLine("{0, -20}", "3. Member Name");
			Console.WriteLine("{0, -20}", "4. Member National Code");
			Console.WriteLine("{0, -20}", "5. Status (Active/Returned)");
			Console.WriteLine("6. Back");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 7);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinBookNameLength,
							ValidationConstants.MaxBookNameLength), "Enter a book title to search",
						loan => loan.Book.Title,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly,
						loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book ISBN to search",
						loan => loan.Book.ISBN,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly,
						loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinNameLength,
							ValidationConstants.MaxNameLength), "Enter a member name to search",
						loan => $"{loan.User.FirstName} {loan.User.LastName}",
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly,
						loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					SearchLoanAndDisplay(ConsoleHelper.GetValidNationalCode,
						"Enter a member national code to search",
						loan => loan.User.NationalCode,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					SearchLoanAndDisplay(ConsoleHelper.ReadLoanStatus, "Enter loan status",
						loan => loan.Status, (search, value) => search == value,
						activeOnly, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Loan"));
					return;
				}
			}
		}
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, bool activeOnly, LoanManagementService loanManagementService,
		ICurrentUserSession session)
		where T : class
	{
		var searchTerm = reader(prompt);
		if (searchTerm is null) return;

		var results = activeOnly
			? loanManagementService.SearchLoans(searchTerm, selector, comparer, session, LoanFilter.Active)
			: loanManagementService.SearchLoans(searchTerm, selector, comparer, session, LoanFilter.All);

		DisplayLoanResults(results);
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, bool activeOnly, LoanManagementService loanManagementService,
		ICurrentUserSession session)
		where T : struct
	{
		var searchTerm = reader(prompt);
		if (!searchTerm.HasValue) return;

		var results = activeOnly
			? loanManagementService.SearchLoans(searchTerm.Value, selector, comparer, session, LoanFilter.Active)
			: loanManagementService.SearchLoans(searchTerm.Value, selector, comparer, session, LoanFilter.All);
		DisplayLoanResults(results);
	}


	private static void DisplayLoanResults(IReadOnlyList<LoanDto> results)
	{
		if (results.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotLoanMatched);
			return;
		}

		Console.Clear();
		LoanPrinter.PrintTable(results);
	}


	private static void ViewLoans(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization, LibraryStatisticsService statisticsService)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.MyActiveLoans,
			    Permission.ViewActiveLoans,
			    Permission.MyOverdueLoans,
			    Permission.ViewOverdueLoans,
			    Permission.MyInactiveLoans,
			    Permission.ViewInactiveLoans,
			    Permission.ViewAllLoans))
			return;

		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);

			var actionId = ShowViewLoanMenu(authorization);
			switch (actionId)
			{
				case 1:
					Console.Clear();
					BrowseLoansByFilter(loanManagementService, userManagementService, bookManagementService, session,
						authorization, LoanFilter.Active, ownPermission: Permission.MyActiveLoans,
						byUserPermission: Permission.ViewActiveLoansByUser,
						byBookPermission: Permission.ViewActiveLoansByBook,
						menuTitle: "ACTIVE LOANS", emptyMessage: Messages.NoActiveLoans);
					ConsoleHelper.Pause();
					break;

				case 2:
					Console.Clear();
					BrowseLoansByFilter(loanManagementService, userManagementService, bookManagementService,
						session, authorization, LoanFilter.Overdue, ownPermission: Permission.MyOverdueLoans,
						byUserPermission: Permission.ViewOverdueLoansByUser,
						byBookPermission: Permission.ViewOverdueLoansByBook,
						menuTitle: "OVERDUE LOANS", emptyMessage: Messages.NoOverdueLoans);
					ConsoleHelper.Pause();
					break;

				case 3:
					Console.Clear();
					BrowseLoansByFilter(loanManagementService, userManagementService, bookManagementService,
						session, authorization, LoanFilter.Inactive,
						ownPermission: Permission.MyInactiveLoans,
						byUserPermission: Permission.ViewInactiveLoansByUser,
						byBookPermission: Permission.ViewInactiveLoansByBook,
						menuTitle: "INACTIVE LOANS", emptyMessage: Messages.NoInactiveLoans);
					ConsoleHelper.Pause();
					break;

				case 4:
					Console.Clear();
					BrowseLoansByFilter(loanManagementService, userManagementService, bookManagementService,
						session, authorization, LoanFilter.All, ownPermission: null,
						byUserPermission: Permission.ViewAllLoans, byBookPermission: Permission.ViewAllLoans,
						menuTitle: "ALL LOANS", emptyMessage: Messages.NotAvailableLoan,
						allowFullLibrary: authorization.HasPermission(Permission.ViewAllLoans));
					ConsoleHelper.Pause();
					break;

				case 5:
					ConsoleHelper.ShowInfo(Messages.BackToLoanMenu);
					return;
			}
		}
	}


	private static int ShowViewLoanMenu(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string Text, bool Available)>
		{
			(1, "View Active Loans",
				authorization.HasAnyPermission(Permission.MyActiveLoans, Permission.ViewActiveLoans,
					Permission.ViewActiveLoansByUser, Permission.ViewActiveLoansByBook)),
			(2, "View Overdue Loans",
				authorization.HasAnyPermission(Permission.MyOverdueLoans, Permission.ViewOverdueLoans,
					Permission.ViewOverdueLoansByUser, Permission.ViewOverdueLoansByBook)),
			(3, "View Inactive Loans",
				authorization.HasAnyPermission(Permission.MyInactiveLoans, Permission.ViewInactiveLoans,
					Permission.ViewInactiveLoansByUser, Permission.ViewInactiveLoansByBook)),
			(4, "View All Loans", authorization.HasPermission(Permission.ViewAllLoans)),
			(5, "Back", true)
		};

		var available = items.Where(i => i.Available).ToList();
		Console.WriteLine(new string('=', 35) + " VIEW LOAN MENU " + new string('=', 35));
		for (var i = 0; i < available.Count; i++) Console.WriteLine($"{i + 1}. {available[i].Text}");
		Console.WriteLine(new string('=', 82));

		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, available.Count, allowCancel: false);
		return available[choice!.Value - 1].ActionId;
	}


	private static void BrowseLoansByFilter(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization, LoanFilter filter, Permission? ownPermission,
		Permission byUserPermission, Permission byBookPermission, string menuTitle, string emptyMessage,
		bool allowFullLibrary = false)
	{
		var canOwn = ownPermission is not null && authorization.HasPermission(ownPermission.Value);
		var canByUser = authorization.HasPermission(byUserPermission);
		var canByBook = authorization.HasPermission(byBookPermission);
		var canFull = allowFullLibrary && authorization.HasPermission(Permission.ViewAllLoans);

		switch (canOwn)
		{
			case false when !canByUser && !canByBook && !canFull:
				ConsoleHelper.ShowError(Messages.AccessDenied);
				return;
			case true when !canByUser && !canByBook && !canFull:
				DisplayLoans(loanManagementService.GetAllLoans(session, filter), emptyMessage);
				return;
		}

		var items = new List<(int ActionId, string Text, bool Available)>
		{
			(1, "Own loans", canOwn),
			(2, "By user", canByUser),
			(3, "By book", canByBook),
			(4, "Full library", canFull),
			(5, "Back", true)
		};
		var available = items.Where(i => i.Available).ToList();

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + $" {menuTitle} " + new string('=', 36));
			for (var i = 0; i < available.Count; i++) Console.WriteLine($"{i + 1}. {available[i].Text}");
			Console.WriteLine(new string('=', 82));

			var choice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, available.Count);
			if (choice is null) return;

			switch (available[choice.Value - 1].ActionId)
			{
				case 1:
					Console.Clear();
					DisplayLoans(loanManagementService.GetAllLoans(session, filter), emptyMessage);
					ConsoleHelper.Pause();
					break;
				case 2:
					Console.Clear();
					ViewLoansByUser(loanManagementService, userManagementService, session, filter, emptyMessage);
					ConsoleHelper.Pause();
					break;
				case 3:
					Console.Clear();
					ViewLoansByBook(loanManagementService, bookManagementService, session, filter);
					ConsoleHelper.Pause();
					break;
				case 4:
					Console.Clear();
					DisplayLoans(loanManagementService.GetAllLoans(session, LoanFilter.All), emptyMessage);
					ConsoleHelper.Pause();
					break;
				case 5:
					return;
			}
		}
	}


	private static void ViewLoansByUser(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		LoanFilter filter, string emptyMessage)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
			users => UserPrinter.PrintTable(users));
		if (user is null) return;

		DisplayLoans(loanManagementService.GetLoansByUser(user.Id, session, filter),
			emptyMessage);
	}


	private static void ViewLoansByBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, ICurrentUserSession session, LoanFilter filter)
	{
		var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
		if (book is null) return;

		DisplayLoans(loanManagementService.GetLoanByBook(book.Id, session, filter),
			Messages.NotAvailableLoan);
	}


	private static void DisplayLoans(IReadOnlyList<LoanDto> loans, string emptyMessage)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		Console.Clear();
		LoanPrinter.PrintTable(loans);
	}
}