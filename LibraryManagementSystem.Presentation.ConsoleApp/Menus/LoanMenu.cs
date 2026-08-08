using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
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
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.BorrowBook,
			    Permission.ReturnBook,
			    Permission.RenewLoan,
			    Permission.ViewBorrowedBooks,
			    Permission.ViewLoanHistory,
			    Permission.ViewOverdueLoans,
			    Permission.ViewUserLoans,
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

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			switch (LoanMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.BorrowBook, Messages.AccessDenied))
						break;
					BorrowBook(loanManagementService, bookManagementService, userManagementService, session);
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ReturnBook, Messages.AccessDenied))
						break;
					ReturnBook(loanManagementService, userManagementService, session);
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RenewLoan, Messages.AccessDenied))
						break;
					RenewLoan(loanManagementService, userManagementService, session);
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewActiveLoans,
						    Messages.AccessDenied))
						break;
					var loans = loanManagementService.GetAllActiveLoans(session);
					DisplayLoans(loans, Messages.NoActiveLoans);
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewActiveLoansByBook,
						    Messages.AccessDenied))
						break;
					var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks());
					if (book is null) break;
					var loans = loanManagementService.GetActiveLoansByBook(book.BookId);
					DisplayLoans(loans, Messages.NotAvailableLoan);
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewActiveLoansByUser,
						    Messages.AccessDenied))
						break;
					int userId;
					if (session.IsSelfServiceMember)
						userId = session.UserId!.Value;
					else
					{
						var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
						if (user is null) break;
						userId = user.Id;
					}

					var result = loanManagementService.GetActiveLoansByUser(userId, session);
					if (!result.Success)
					{
						ConsoleHelper.ShowError(result.Message!);
						break;
					}

					DisplayLoans(result.Data ?? [], Messages.UserHasNoBorrowedBooks);
					break;
				}
				case 7:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewOverdueLoans,
						    Messages.AccessDenied))
						break;
					var loans = loanManagementService.GetOverdueLoans(session);
					DisplayLoans(loans, Messages.NoOverdueLoans);
					break;
				}
				case 8:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.HistoryByBook, Messages.AccessDenied))
						break;
					var book = MenuHelper.SelectBook(bookManagementService.GetAllBooks());
					if (book is null) break;

					var loans = loanManagementService.GetLoanByBook(book.BookId);
					DisplayLoans(loans, Messages.NotAvailableLoan);
					break;
				}
				case 9:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.HistoryByUser, Messages.AccessDenied))
						break;
					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
					if (user is null) break;

					DisplayLoans(loanManagementService.GetLoansByUser(user.Id, session),
						Messages.UserHasNoBorrowedBooks);
					break;
				}
				case 10:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.FullLibraryHistory,
						    Messages.AccessDenied))
						break;
					var allLoans = loanManagementService.GetFullLibraryHistory();
					DisplayLoans(allLoans, Messages.NotAvailableLoan);
					break;
				}
				case 11:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchLoans, Messages.AccessDenied))
						break;
					SearchLoan(loanManagementService, session);
					break;
				}
				case 12:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static int LoanMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Borrow Book", authorization.HasPermission(Permission.BorrowBook)),
			(2, "Return Book", authorization.HasPermission(Permission.ReturnBook)),
			(3, "Renew Loan", authorization.HasPermission(Permission.RenewLoan)),
			(4, "My Active Loans", authorization.HasPermission(Permission.ViewActiveLoans)),
			(5, "Active Loans by Book", authorization.HasPermission(Permission.ViewActiveLoansByBook)),
			(6, "Active Loans by User", authorization.HasPermission(Permission.ViewActiveLoansByUser)),
			(7, "Overdue Loans", authorization.HasPermission(Permission.ViewOverdueLoans)),
			(8, "Full History By Book", authorization.HasPermission(Permission.HistoryByBook)),
			(9, "Full History By User", authorization.HasPermission(Permission.HistoryByUser)),
			(10, "Full  Library History", authorization.HasPermission(Permission.FullLibraryHistory)),
			(11, "Search Loans", authorization.HasPermission(Permission.SearchLoans)),
			(12, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " LOAN MENU " + new string('=', 36));

			var displayNumber = 1;
			foreach (var item in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {item.DisplayText}");
				displayNumber++;
			}

			Console.WriteLine(new string('=', 83));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count) return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
	}


	private static void BorrowBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, UserManagementService userManagementService,
		ICurrentUserSession session)
	{
		int userId;
		if (session.IsSelfServiceMember)
		{
			userId = session.UserId!.Value;
		}
		else
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
			if (user is null)
			{
				ConsoleHelper.ShowWarning(Messages.UserNotFound);
				return;
			}

			userId = user.Id;
		}

		var availableBooks = bookManagementService.GetAvailableBooks();
		if (availableBooks.Count is 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableBookToBorrow);
			return;
		}

		BookPrinter.PrintTable(availableBooks);
		int bookId;
		while (true)
		{
			var input = ConsoleHelper.ReadInt("Enter your desired book id to borrow", 1, int.MaxValue);
			if (input is null) return;
			var selectedBook = availableBooks.FirstOrDefault(b => b.BookId == input);
			if (selectedBook is null)
			{
				ConsoleHelper.ShowError(Messages.InvalidBookSelection);
				continue;
			}

			bookId = (int)input;
			break;
		}


		var result = loanManagementService.BorrowBook(userId, bookId, session);
		ConsoleHelper.ShowResult(result);
	}


	private static LoanDto? SelectActiveLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, string purpose)
	{
		int userId;
		if (session.IsSelfServiceMember)
		{
			userId = session.UserId!.Value;
		}
		else
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
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



	private static void DisplayLoans(IReadOnlyList<LoanDto> loans, string emptyMessage)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		LoanPrinter.PrintTable(loans);
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
				ConsoleHelper.ShowWarning(activeOnly
					? Messages.NotAvailableActionLoan
					: Messages.NotAvailableLoan);

				ConsoleHelper.Pause();
				return;
			}

			Console.WriteLine("{0, -20}", "1. Loan ID");
			Console.WriteLine("{0, -20}", "2. Book Title");
			Console.WriteLine("{0, -20}", "3. Book ISBN");
			Console.WriteLine("{0, -20}", "4. Member Name");
			Console.WriteLine("{0, -20}", "5. Member National Code");
			Console.WriteLine("{0, -20}", "6. Status (Active/Returned)");
			Console.WriteLine("7. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 7);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadInt(p, 1, int.MaxValue),
						"Enter an ID to search", loan => loan.LoanId, (search, value) => search == value,
						activeOnly, loanManagementService, session);

					break;
				}
				case 2:
				{
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinBookNameLength,
							ValidationConstants.MaxBookNameLength), "Enter a book title to search",
						loan => loan.Book.BookName,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 3:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book ISBN to search",
						loan => loan.Book.InternationalStandardBookNumber,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 4:
				{
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinNameLength,
							ValidationConstants.MaxNameLength), "Enter a member name to search",
						loan => $"{loan.User.FirstName} {loan.User.LastName}",
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 5:
				{
					SearchLoanAndDisplay(ConsoleHelper.GetValidNationalCode, "Enter a member national code to search",
						loan => loan.User.NationalCode,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly, loanManagementService, session);

					break;
				}
				case 6:
				{
					SearchLoanAndDisplay(ConsoleHelper.ReadLoanStatus, "Enter loan status",
						loan => loan.Status, (search, value) => search == value,
						activeOnly, loanManagementService, session);

					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Loan"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
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

		LoanPrinter.PrintTable(results);
	}
}