using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoanMenu
{
	public static void LoanMenuController(LoanManagementService loanManagementService,
		UserManagementService userManagementService, BookManagementService bookManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session)
	{
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
			switch (LoanMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					BorrowBook(loanManagementService, bookManagementService, userManagementService, session);
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
					Console.Clear();
					ViewBorrowedBooks(loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					DisplayLoansForUsers(loanManagementService, userManagementService, activeOnly: false,
						Messages.NoLoanHistoryForUser, session);

					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					ViewOverdueLoans(loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					DisplayLoansForUsers(loanManagementService, userManagementService, activeOnly: true,
						Messages.UserHasNoBorrowedBooks, session);

					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					if (!SessionGuard.RequireAdminOrLibrarian(session)) break;
					SearchLoan(loanManagementService, session);
					break;
				}
				case 9:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					ConsoleHelper.Pause();
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int LoanMenuList(ICurrentUserSession session) {
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Borrow Book", true),
			(2, "Return Book", true),
			(3, "Renew Loan", true),
			(4, "View Borrowed Books", true),
			(5, "View Loan History", true),
			(6, "View Overdue Loans", true),
			(7, "View User Loans", session.IsAdmin || session.IsLibrarian),
			(8, "Search Loans", session.IsAdmin || session.IsLibrarian),
			(9, "Back", true)
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

			Console.WriteLine(new string('=', 82));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count)
				return availableItems[userChoice - 1].ActionId;

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


	private static void ViewBorrowedBooks(LoanManagementService loanManagementService, ICurrentUserSession session)
	{
		IReadOnlyList<LoanDto> loans;
		if (session.IsSelfServiceMember)
		{
			var result = loanManagementService.GetActiveLoansByUser(session.UserId!.Value, session);
			if (!result.Success)
			{
				ConsoleHelper.ShowError(result.Message!);
				return;
			}

			loans = result.Data ?? [];
		}
		else
		{
			loans = loanManagementService.GetAllActiveLoans(session);
		}

		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(Messages.NoActiveLoans);
			return;
		}

		LoanPrinter.PrintTable(loans);
	}


	private static void DisplayLoansForUsers(LoanManagementService loanManagementService,
		UserManagementService userManagementService, bool activeOnly, string emptyMessage, ICurrentUserSession session)
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

		IReadOnlyList<LoanDto> loans;
		if (activeOnly)
		{
			var result = loanManagementService.GetActiveLoansByUser(userId, session);
			if (!result.Success)
			{
				ConsoleHelper.ShowError(result.Message!);
				return;
			}

			loans = result.Data ?? [];
		}
		else
		{
			var result = loanManagementService.GetLoansByUser(userId, session);
			if (!result.Success)
			{
				ConsoleHelper.ShowError(result.Message!);
				return;
			}

			loans = result.Data ?? [];
		}

		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		LoanPrinter.PrintTable(loans);
	}


	private static void ViewOverdueLoans(LoanManagementService loanManagementService, ICurrentUserSession session)
	{
		IReadOnlyList<LoanDto> loans;
		if (session.IsSelfServiceMember)
		{
			var result = loanManagementService.GetActiveLoansByUser(session.UserId!.Value, session);
			if (!result.Success)
			{
				ConsoleHelper.ShowError(result.Message!);
				return;
			}

			loans = (result.Data ?? []).Where(l => l.IsOverdue).ToList().AsReadOnly();
		}
		else
		{
			loans = loanManagementService.GetOverdueLoans(session);
		}

		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(Messages.NoOverdueLoans);
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
						loan => loan.BookName,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 3:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book ISBN to search",
						loan => loan.BookISBN,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 4:
				{
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinNameLength,
							ValidationConstants.MaxNameLength), "Enter a member name to search",
						loan => loan.UserName,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase), activeOnly,
						loanManagementService, session);

					break;
				}
				case 5:
				{
					SearchLoanAndDisplay(ConsoleHelper.GetValidNationalCode, "Enter a member national code to search",
						loan => loan.UserNationalCode,
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


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<LoanDto, T?> selector,
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


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<LoanDto, T?> selector,
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