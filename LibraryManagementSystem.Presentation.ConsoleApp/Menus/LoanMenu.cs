using LibraryManagementSystem.Application.Authorization;
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
			    Permission.ViewLoansByUser,
			    Permission.ViewLoansByBook,
			    Permission.SearchLoans))
			return;


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
					Console.Clear();
					BorrowBook(loanManagementService, bookManagementService, userManagementService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ReturnBook(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
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
			(1, "Borrow Book", true),
			(2, "Return Book", true),
			(3, "Renew Loan", true),
			(4, "Search Loans", authorization.HasPermission(Permission.SearchLoans)),
			(5, "View Loans", true),
			(6, "Back", true)
		};

		return MenuHelper.DisplayMenu("LOAN MENU", items);
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


	private static void ReturnBook(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = MenuHelper.SelectActiveLoan(loanManagementService, userManagementService, session, "return");
		if (loan is null) return;

		var result = loanManagementService.ReturnBook(loan.Id, session);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = MenuHelper.SelectActiveLoan(loanManagementService, userManagementService, session, "renew");
		if (loan is null) return;

		var result = loanManagementService.RenewLoan(loan, session);
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
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied, Permission.ViewLoansByUser,
			    Permission.ViewLoansByBook, Permission.ViewAllLoans))
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
					BrowseMyLoans(loanManagementService, session, authorization);
					break;
				case 2:
					BrowseLoansByUser(loanManagementService, userManagementService, session, authorization);
					break;
				case 3:
					BrowseLoansByBook(loanManagementService, bookManagementService, session, authorization);
					break;
				case 4:
					Console.Clear();
					DisplayLoans(loanManagementService.GetAllLoans(session, LoanFilter.All),
						Messages.NotAvailableLoan);
					break;
				case 5:
					ConsoleHelper.ShowInfo(Messages.BackToLoanMenu);
					return;
			}

			ConsoleHelper.Pause();
		}
	}


	private static int ShowViewLoanMenu(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string Text, bool Available)>
		{
			(1, "My loans", true),
			(2, "By user", authorization.HasPermission(Permission.ViewLoansByUser)),
			(3, "By book", authorization.HasPermission(Permission.ViewLoansByBook)),
			(4, "Full library", authorization.HasPermission(Permission.ViewAllLoans)),
			(5, "Back", true)
		};

		return MenuHelper.DisplayMenu("VIEW LOAN MENU", items);
	}


	private static LoanFilter? PickLoanFilter(IAuthorizationService authorization, Permission? filterPermission, bool allowAll)
	{
		var canView = filterPermission is null || authorization.HasPermission(filterPermission.Value);

		var items = new List<(LoanFilter? Filter, string Text, bool Available)>
		{
			(LoanFilter.Active, "Active", canView),
			(LoanFilter.Overdue, "Overdue", canView),
			(LoanFilter.Inactive, "Inactive", canView),
			(LoanFilter.All, "All", allowAll && authorization.HasPermission(Permission.ViewAllLoans)),
			(null, "Back", true)
		};

		return MenuHelper.DisplayMenu("LOAN FILTER", items);
	}


	private static void BrowseMyLoans(LoanManagementService loanManagementService, ICurrentUserSession session,
		IAuthorizationService authorization)
	{
		var filter = PickLoanFilter(authorization, null, allowAll: false);
		if (filter is null) return;

		Console.Clear();
		DisplayLoans(loanManagementService.GetAllLoans(session, filter.Value),
			filter.Value switch
			{
				LoanFilter.Active => Messages.NoActiveLoans,
				LoanFilter.Overdue => Messages.NoOverdueLoans,
				LoanFilter.Inactive => Messages.NoInactiveLoans,
				_ => Messages.NotAvailableLoan
			});
	}


	private static void BrowseLoansByUser(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
			users => UserPrinter.PrintTable(users));
		if (user is null) return;

		var filter = PickLoanFilter(authorization, Permission.ViewLoansByUser, allowAll: true);
		if (filter is null) return;

		Console.Clear();
		DisplayLoans(loanManagementService.GetLoansByUser(user.Id, session, filter.Value),
			Messages.NotAvailableLoan);
	}


	private static void BrowseLoansByBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
		if (book is null) return;

		var filter = PickLoanFilter(authorization, Permission.ViewLoansByBook, allowAll: true);
		if (filter is null) return;

		Console.Clear();
		DisplayLoans(loanManagementService.GetLoanByBook(book.Id, session, filter.Value), Messages.NotAvailableLoan);
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