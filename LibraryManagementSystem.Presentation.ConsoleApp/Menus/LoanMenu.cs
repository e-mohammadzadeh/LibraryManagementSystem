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
	public static void LoanMenuController(LoanManagementService loanManagementService, IMenuService menuService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.ViewLoan,
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
			switch (LoanMenuList(menuService))
			{
				case "loans.borrow":
				{
					Console.Clear();
					BorrowBook(loanManagementService, bookManagementService, userManagementService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.return":
				{
					Console.Clear();
					ReturnBook(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.renew":
				{
					Console.Clear();
					RenewLoan(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.search":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchLoans, Messages.AccessDenied))
						break;
					Console.Clear();
					SearchLoan(loanManagementService, session, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.view":
				{
					Console.Clear();
					ViewLoans(loanManagementService, menuService, userManagementService, bookManagementService, session,
						authorization, statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
		}
	}



	private static string? LoanMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("loans");
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
		IMenuService menuService, bool activeOnly = false)
	{
		var loanList = activeOnly
			? loanManagementService.GetAllLoans(session, LoanFilter.Active)
			: loanManagementService.GetAllLoans(session, LoanFilter.All);

		if (loanList.Count == 0)
		{
			ConsoleHelper.ShowWarning(activeOnly ? Messages.NotAvailableActionLoan : Messages.NotAvailableLoan);
			return;
		}

		while (true)
		{
			Console.Clear();
			var selectedId = MenuHelper.DisplayMenu("SEARCHING LOAN MENU", menuService.GetChildren("loans.search"));
			if (selectedId is null) return;

			if (selectedId.Equals("loans.search.back", StringComparison.OrdinalIgnoreCase))
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Loan"));
				return;
			}

			switch (selectedId.ToLowerInvariant())
			{
				case "loans.search.title":
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
				case "loans.search.isbn":
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
				case "loans.search.name":
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
				case "loans.search.national-code":
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
				case "loans.search.status":
				{
					Console.Clear();
					SearchLoanAndDisplay(ConsoleHelper.ReadLoanStatus, "Enter loan status",
						loan => loan.Status, (search, value) => search == value,
						activeOnly, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "loans.search.back":
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


	private static void ViewLoans(LoanManagementService loanManagementService, IMenuService menuService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization, LibraryStatisticsService statisticsService)
	{
		if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied, Permission.ViewLoan,
			    Permission.ViewLoansByUser, Permission.ViewLoansByBook, Permission.ViewAllLoans))
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

			var actionId = ShowViewLoanMenu(menuService);
			switch (actionId)
			{
				case "loans.view.own":
					BrowseMyLoans(loanManagementService, session, menuService);
					break;
				case "loans.view.by-user":
					BrowseLoansByUser(loanManagementService, userManagementService, session, menuService);
					break;
				case "loans.view.by-book":
					BrowseLoansByBook(loanManagementService, bookManagementService, session, menuService);
					break;
				case "loans.view.full":
					Console.Clear();
					DisplayLoans(loanManagementService.GetAllLoans(session, LoanFilter.All),
						Messages.NotAvailableLoan);
					break;
				case "loans.view.back":
					ConsoleHelper.ShowInfo(Messages.BackToLoanMenu);
					return;
			}

			ConsoleHelper.Pause();
		}
	}


	private static string? ShowViewLoanMenu(IMenuService menuService)
	{
		var items = menuService.GetChildren("loans.view");
		return MenuHelper.DisplayMenu("VIEW LOAN MENU", items);
	}


	private static LoanFilter? PickLoanFilter(IMenuService menuService)
	{
		var selectedItem = MenuHelper.DisplayMenu("LOAN FILTER", menuService.GetChildren("loans.filter"));
		return selectedItem switch
		{
			"loans.filter.active" => LoanFilter.Active,
			"loans.filter.overdue" => LoanFilter.Overdue,
			"loans.filter.inactive" => LoanFilter.Inactive,
			"loans.filter.all" => LoanFilter.All,
			_ => null
		};
	}


	private static void BrowseMyLoans(LoanManagementService loanManagementService, ICurrentUserSession session,
		IMenuService menuService)
	{
		var filter = PickLoanFilter(menuService);
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
		UserManagementService userManagementService, ICurrentUserSession session, IMenuService menuService)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
			users => UserPrinter.PrintTable(users));
		if (user is null) return;

		var filter = PickLoanFilter(menuService);
		if (filter is null) return;

		Console.Clear();
		DisplayLoans(loanManagementService.GetLoansByUser(user.Id, session, filter.Value),
			Messages.NotAvailableLoan);
	}


	private static void BrowseLoansByBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, ICurrentUserSession session, IMenuService menuService)
	{
		var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
		if (book is null) return;

		var filter = PickLoanFilter(menuService);
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