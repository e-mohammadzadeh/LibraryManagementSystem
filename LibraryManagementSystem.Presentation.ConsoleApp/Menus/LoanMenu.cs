using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
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
			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
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
					ViewBorrowedBooks(loanManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					DisplayLoansForUsers(userManagementService, loanManagementService.GetLoansByUser,
						ValidationMessages.NoLoanHistoryForUser);

					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					var loans = loanManagementService.GetOverdueLoans();
					if (loans.Count is 0)
					{
						ConsoleHelper.ShowWarning(ValidationMessages.NoOverdueLoans);
						ConsoleHelper.Pause();
						break;
					}

					LoanPrinter.PrintTable(loans);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					DisplayLoansForUsers(userManagementService, loanManagementService.GetActiveLoansByUser,
						ValidationMessages.UserHasNoBorrowedBooks);

					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					SearchLoan(loanManagementService);
					break;
				}
				case 9:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int LoanMenuList(ICurrentUserSession session)
	{
		while (true)
		{
			Console.WriteLine(new string('=', 36) + " LOAN MENU " + new string('=', 36));
			Console.WriteLine("1. Borrow Book");
			Console.WriteLine("2. Return Book");
			Console.WriteLine("3. Renew Loan");
			Console.WriteLine("4. View Borrowed Books");
			Console.WriteLine("5. View Loan History");
			Console.WriteLine("6. View Overdue Loans");

			if (session.IsAdmin || session.IsLibrarian)
			{
				Console.WriteLine("7. View User Loans");
				Console.WriteLine("8. Search Loans");
			}

			Console.WriteLine("9. Back");
			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var result))
			{
				ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
				continue;
			}

			switch (result)
			{
				case 1 or 2 or 3 or 4 or 5 or 6 or 9:
				case 7 or 8 when (session.IsAdmin || session.IsLibrarian):
					return result;
				default:
					ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
					break;
			}
		}
	}


	private static void BorrowBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService, UserManagementService userManagementService, ICurrentUserSession session)
	{
		int userId;
		if (session.IsMember && !session.IsAdmin && !session.IsLibrarian)
		{
			userId = session.UserId!.Value;
		}
		else
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers());
			if (user is null)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.UserNotFound);
				return;
			}
			userId = user.Id;
		}

		var availableBooks = bookManagementService.GetAvailableBooks();
		if (availableBooks.Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook2Borrow);
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
				ConsoleHelper.ShowError(
					"That book ID is not in the available list. Please select from the list above.");

				continue;
			}

			bookId = (int)input;
			break;
		}


		var result = loanManagementService.BorrowBook((int)userId, bookId);
		ConsoleHelper.ShowResult(result);
	}


	private static LoanDto? SelectActiveLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, string purpose)
	{
		int userId;
		if (session.IsMember && !session.IsAdmin && !session.IsLibrarian)
		{
			userId = session.UserId!.Value;
		}
		else
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers());
			if (user is null)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.UserNotFound);
				return null;
			}

			userId = user.Id;
		}

		var loans = loanManagementService.GetActiveLoansByUser((int)userId);
		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UserHasNoBorrowedBooks);
			return null;
		}

		LoanPrinter.PrintTable(loans);
		var loanId = ConsoleHelper.ReadInt($"Enter loan id to {purpose}", 1, int.MaxValue);
		if (loanId is null) return null;

		var selectedLoan = loans.FirstOrDefault(l => l.LoanId == loanId);
		if (selectedLoan is not null) return selectedLoan;

		ConsoleHelper.ShowError(ValidationMessages.ActiveLoanNotFound);
		return null;
	}


	private static void ReturnBook(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "return");
		if (loan is null) return;

		var result = loanManagementService.ReturnBook(loan.LoanId);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var loan = SelectActiveLoan(loanManagementService, userManagementService, session, "renew");
		if (loan is null) return;

		var result = loanManagementService.RenewLoan(loan.LoanId);
		ConsoleHelper.ShowResult(result);
	}


	private static void ViewBorrowedBooks(LoanManagementService loanManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{



		var loans = loanManagementService.GetAllActiveLoans();
		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NoActiveLoans);
			ConsoleHelper.Pause();
			break;
		}

		LoanPrinter.PrintTable(loans);
	}


	private static void DisplayLoansForUsers(UserManagementService userManagementService,
		Func<int, IReadOnlyList<LoanDto>> getLoansFn, string emptyMessage)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers());
		if (user is null)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UserNotFound);
			return;
		}

		var loans = getLoansFn(user.Id);
		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		LoanPrinter.PrintTable(loans);
	}


	private static void SearchLoan(LoanManagementService loanManagementService, bool activeOnly = true)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING LOAN MENU " + new string('=', 36));
			var loanList = activeOnly ? loanManagementService.GetAllActiveLoans() : loanManagementService.GetAllLoans();
			if (loanList.Count is 0)
			{
				ConsoleHelper.ShowWarning(activeOnly
					? ValidationMessages.NotAvailableActionLoan
					: ValidationMessages.NotAvailableLoan);

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

			var searchMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.SearchMenuQuestion, 1, 7);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadInt(p, 1, int.MaxValue),
						"Enter an ID to search", loan => loan.LoanId, (search, value) => search == value,
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 2:
				{
					SearchLoanAndDisplay(
						p => ConsoleHelper.GetValidName(p, ValidationConstants.MinBookNameLength,
							ValidationConstants.MaxBookNameLength), "Enter a book title to search",
						loan => loan.Book.BookName, (search, value) => value.Contains(search,
							StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 3:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book ISBN to search",
						loan => loan.Book.InternationalStandardBookNumber,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 4:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.GetValidName(p, ValidationConstants.MinNameLength,
							ValidationConstants.MaxNameLength), "Enter a member name to search",
						loan => $"{loan.User.FirstName} {loan.User.LastName}",
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 5:
				{
					SearchLoanAndDisplay(ConsoleHelper.GetValidNationalCode, "Enter a member national code to search",
						loan => loan.User.NationalCode,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 6:
				{
					SearchLoanAndDisplay(ConsoleHelper.ReadLoanStatus, "Enter loan status",
						loan => loan.Status, (search, value) => search == value,
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo("Search cancelled. Returning to Loan Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, Func<T?, Func<Loan, T?>, Func<T, T, bool>, IReadOnlyList<LoanDto>> searchFn)
		where T : class
	{
		var searchTerm = reader(prompt);
		if (searchTerm is null) return;

		var results = searchFn(searchTerm, selector, comparer);
		DisplayLoanResults(results);
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, Func<T?, Func<Loan, T?>, Func<T, T, bool>, IReadOnlyList<LoanDto>> searchFn)
		where T : struct
	{
		var searchTerm = reader(prompt);
		if (!searchTerm.HasValue) return;

		var results = searchFn(searchTerm.Value, selector, comparer);
		DisplayLoanResults(results);
	}


	private static void DisplayLoanResults(IReadOnlyList<LoanDto> results)
	{
		if (results.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotLoanMatched);
			return;
		}

		LoanPrinter.PrintTable(results);
	}
}