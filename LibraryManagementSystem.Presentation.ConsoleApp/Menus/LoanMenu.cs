using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoanMenu
{
	public static void LoanMenuController(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization,
		LoanHistoryManagementService loanHistoryManagementService)
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
					Console.Clear();
					ActiveLoans(loanManagementService, userManagementService, bookManagementService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					OverdueLoans(loanManagementService, userManagementService, bookManagementService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchLoans, Messages.AccessDenied))
						break;
					Console.Clear();
					SearchLoan(loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					History(loanManagementService, userManagementService, bookManagementService, session,
						authorization, loanHistoryManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 8:
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
			(4, "Active Loans", authorization.HasAnyPermission(Permission.MyActiveLoans, Permission.ViewActiveLoans)),
			(5, "Overdue Loans",
				authorization.HasAnyPermission(Permission.MyOverdueLoans, Permission.ViewOverdueLoans)),
			(6, "Search Loans", authorization.HasPermission(Permission.SearchLoans)),
			(7, "History", authorization.HasAnyPermission(Permission.ViewLoanHistory, Permission.MyFullLoanHistory)),
			(8, "Back", true)
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
		int userId;
		UserDto? user;
		if (session.IsSelfServiceMember)
		{
			userId = session.UserId!.Value;
			user = userManagementService.FindUserById(userId);
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

		var availableBooks = bookManagementService.GetAvailableBooks();
		if (availableBooks.Count is 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableBookToBorrow);
			return;
		}

		BookPrinter.PrintTable(availableBooks, authorization);
		int bookId;
		while (true)
		{
			var input = ConsoleHelper.ReadInt("Enter your desired book id to borrow", 1, int.MaxValue);
			if (input is null) return;
			var selectedBook = availableBooks.FirstOrDefault(b => b.BookId == input);
			if (selectedBook is null)
			{
				ConsoleHelper.ShowError(string.Format(Messages.InvalidIdSelection, ""));
				continue;
			}

			bookId = (int)input;
			break;
		}

		var dto = new CreateLoanDto { BookId = bookId, UserId = userId };
		var result = loanManagementService.BorrowBook(dto, session);
		ConsoleHelper.ShowResult(result);
	}


	private static LoanDto? SelectActiveLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, string purpose)
	{
		int userId;
		if (session.IsSelfServiceMember)
			userId = session.UserId!.Value;
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

		var result = loanManagementService.GetActiveLoansByUser(userId, session);
		if (!result.Success)
		{
			ConsoleHelper.ShowError(result.Message!);
			return null;
		}

		var loans = result.Data;
		if (loans is null || loans.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UserHasNoBorrowedBooks);
			return null;
		}

		Console.Clear();
		LoanPrinter.PrintTable(loans);
		var loanId = ConsoleHelper.ReadInt($"Enter loan id to {purpose}", 1, int.MaxValue);
		if (loanId is null) return null;

		var selectedLoan = loans.FirstOrDefault(l => l.LoanId == loanId);
		if (selectedLoan is not null) return selectedLoan;

		ConsoleHelper.ShowError(Messages.ActiveLoanNotFound);
		return null;
	}


	private static void ReturnBook(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "return");
		if (loan is null) return;

		var result = loanManagementService.ReturnBook(loan.LoanId, session);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "renew");
		if (loan is null) return;

		var result = loanManagementService.RenewLoan(loan.LoanId, session);
		ConsoleHelper.ShowResult(result);
	}


	private static void ActiveLoans(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		var canViewOwn = authorization.HasPermission(Permission.MyActiveLoans);
		var canViewByUser = authorization.HasPermission(Permission.ViewActiveLoansByUser);
		var canViewByBook = authorization.HasPermission(Permission.ViewActiveLoansByBook);
		if (!canViewOwn && !canViewByUser && !canViewByBook)
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		if (canViewOwn && !canViewByUser && !canViewByBook)
		{
			DisplayLoans(loanManagementService.GetAllActiveLoans(session), Messages.NoActiveLoans);
			return;
		}

		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Own Active Loans", canViewOwn),
			(2, "Active Loans By User", canViewByUser),
			(3, "Active Loans By Book", canViewByBook),
			(4, "Back", true)
		};
		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " ACTIVE LOAN MENU " + new string('=', 36));
			for (var i = 0; i < availableItems.Count; i++)
				Console.WriteLine($"{i + 1}. {availableItems[i].DisplayText}");
			Console.WriteLine(new string('=', 82));

			var choice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, availableItems.Count);
			if (choice is null) return;

			switch (availableItems[choice.Value - 1].ActionId)
			{
				case 1:
				{
					Console.Clear();
					var loans = loanManagementService.GetAllActiveLoans(session);
					DisplayLoans(loans, Messages.NoActiveLoans);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewActiveLoansByUser(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					ViewActiveLoansByBook(loanManagementService, bookManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					ConsoleHelper.ShowInfo("Backing to Loan Menu");
					return;
				}
			}
		}
	}


	private static void ViewActiveLoansByUser(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(), users => UserPrinter.PrintTable(users));
		if (user is null) return;

		var result = loanManagementService.GetActiveLoansByUser(user.Id, session);
		if (!result.Success)
		{
			ConsoleHelper.ShowError(result.Message!);
			return;
		}

		DisplayLoans(result.Data ?? [], Messages.UserHasNoBorrowedBooks);
	}


	private static void ViewActiveLoansByBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, ICurrentUserSession session)
	{
		var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
		if (book is null) return;
		var loans = loanManagementService.GetActiveLoansByBook(book.BookId, session);
		DisplayLoans(loans, Messages.NotAvailableLoan);
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


	private static void OverdueLoans(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (authorization.HasPermission(Permission.MyOverdueLoans))
		{
			var loans = loanManagementService.GetOverdueLoans(session);
			DisplayLoans(loans, Messages.NoOverdueLoans);
			return;
		}

		if (!authorization.HasAnyPermission(Permission.ViewOverdueLoansByUser, Permission.ViewOverdueLoansByBook))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " OVERDUE LOAN MENU " + new string('=', 36));
		while (true)
		{
			Console.WriteLine("1. Overdue Loans By User");
			Console.WriteLine("2. Overdue Loans By Book");
			Console.WriteLine("3. Back");

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 3);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					if (!authorization.HasPermission(Permission.ViewOverdueLoansByUser))
					{
						ConsoleHelper.ShowError(Messages.AccessDenied);
						break;
					}

					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
						users => UserPrinter.PrintTable(users));
					if (user is null) break;

					var result = loanManagementService.GetOverdueLoansByUser(user.Id, session);
					if (!result.Success)
					{
						ConsoleHelper.ShowError(result.Message!);
						break;
					}

					DisplayLoans(result.Data ?? [], Messages.UserHasNoBorrowedBooks);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					if (!authorization.HasPermission(Permission.ViewOverdueLoansByBook))
					{
						ConsoleHelper.ShowError(Messages.AccessDenied);
						break;
					}

					var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
					if (book is null) break;
					var loans = loanManagementService.GetOverdueLoansByBook(book.BookId, session);
					DisplayLoans(loans, Messages.NotAvailableLoan);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					ConsoleHelper.ShowInfo("Backing to Loan Menu");
					return;
				}
			}
		}
	}



	private static void History(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session, IAuthorizationService authorization,
		LoanHistoryManagementService loanHistoryManagementService)
	{
		var canViewOwn = authorization.HasPermission(Permission.MyFullLoanHistory);
		var canViewByUser = authorization.HasPermission(Permission.LoanHistoryByUser);
		var canViewByBook = authorization.HasPermission(Permission.LoanHistoryByBook);
		var canViewFull = authorization.HasPermission(Permission.FullLibraryHistory);

		var hasNoPermissions = !canViewOwn && !canViewByUser && !canViewByBook && !canViewFull;
		var hasOnlyOwnPermission = canViewOwn && !canViewByUser && !canViewByBook && !canViewFull;

		if (hasNoPermissions)
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		if (hasOnlyOwnPermission)
		{
			if (session.UserId is null)
			{
				ConsoleHelper.ShowError(Messages.AuthenticationRequired);
				return;
			}

			var loans = loanManagementService.GetLoansByUser(session.UserId!.Value, session);
			DisplayLoans(loans, Messages.UserHasNoBorrowedBooks);
			return;
		}

		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "History By User", canViewByUser),
			(2, "History By Book", canViewByBook),
			(3, "Full Library History", canViewFull),
			(4, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " HISTORY MENU " + new string('=', 36));
			for (var i = 0; i < availableItems.Count; i++)
				Console.WriteLine($"{i + 1}. {availableItems[i].DisplayText}");
			Console.WriteLine(new string('=', 82));

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, availableItems.Count);
			if (editMenuChoice is null) return;

			switch (availableItems[editMenuChoice.Value - 1].ActionId)
			{
				case 1:
				{
					Console.Clear();
					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
						users => UserPrinter.PrintTable(users));
					if (user is null) break;

					var histories = loanHistoryManagementService.GetByUserId(user.Id);
					LoanHistoryPrinter.PrintTable(histories, $"Loan History - {user.FullName}");
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks(), "Books List");
					if (book is null) break;

					var histories = loanHistoryManagementService.GetByBookId(book.BookId);
					LoanHistoryPrinter.PrintTable(histories, $"Loan History - {book.BookName}");
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					var histories = loanHistoryManagementService.GetAll();
					LoanHistoryPrinter.PrintTable(histories, "Full Library Loan History");
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					ConsoleHelper.ShowInfo("Backing to Loan Menu");
					return;
				}
			}
		}
	}


	private static void SearchLoan(LoanManagementService loanManagementService, ICurrentUserSession session,
		bool activeOnly = false)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING LOAN MENU " + new string('=', 36));
			var loanList = activeOnly
				? loanManagementService.GetAllActiveLoans(session)
				: loanManagementService.GetAllLoans(session);
			if (loanList.Count == 0)
			{
				ConsoleHelper.ShowWarning(activeOnly ? Messages.NotAvailableActionLoan : Messages.NotAvailableLoan);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Loan ID");
			Console.WriteLine("{0, -20}", "2. Book Title");
			Console.WriteLine("{0, -20}", "3. Book ISBN");
			Console.WriteLine("{0, -20}", "4. Member Name");
			Console.WriteLine("{0, -20}", "5. Member National Code");
			Console.WriteLine("{0, -20}", "6. Status (Active/Returned)");
			Console.WriteLine("7. Back");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 7);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					SearchLoanAndDisplay(p => ConsoleHelper.ReadInt(p, 1, int.MaxValue),
						"Enter an ID to search", loan => loan.LoanId, (search, value) => search == value,
						activeOnly, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinBookNameLength,
							ValidationConstants.MaxBookNameLength), "Enter a book title to search",
						loan => loan.Book.BookName,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly,
						loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book ISBN to search",
						loan => loan.Book.InternationalStandardBookNumber,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly,
						loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
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
				case 5:
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
				case 6:
				{
					Console.Clear();
					SearchLoanAndDisplay(ConsoleHelper.ReadLoanStatus, "Enter loan status",
						loan => loan.Status, (search, value) => search == value,
						activeOnly, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
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
			? loanManagementService.SearchActiveLoans(searchTerm, selector, comparer, session)
			: loanManagementService.SearchLoans(searchTerm, selector, comparer, session);

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
			? loanManagementService.SearchActiveLoans(searchTerm.Value, selector, comparer, session)
			: loanManagementService.SearchLoans(searchTerm.Value, selector, comparer, session);
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
}