using LibraryManagementSystem.Application.Common;
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
		LibraryStatisticsService statisticsService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			StatisticsPrinter.Print(statisticsService.GetLibraryStatistics());
			switch (LoanMenuList())
			{
				case 1:
				{
					Console.Clear();
					BorrowBook(loanManagementService, bookManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					ReturnBook(loanManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					RenewLoan(loanManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 4:
				{
					Console.Clear();
					var loans = loanManagementService.GetAllActiveLoans();
					if (loans.Count is 0)
					{
						ConsoleHelper.ShowWarning(ValidationMessages.NoActiveLoans);
						break;
					}

					LoanPrinter.PrintTable(loans);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 5:
				{
					Console.Clear();
					DisplayLoansForUsers(userManagementService, loanManagementService.GetLoansByUser,
						ValidationMessages.NoLoanHistoryForUser);

					break;
				}
				case 6:
				{
					Console.Clear();
					var loans = loanManagementService.GetOverdueLoans();
					if (loans.Count is 0)
					{
						ConsoleHelper.ShowWarning(ValidationMessages.NoOverdueLoans);
						break;
					}

					LoanPrinter.PrintTable(loans);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					Console.Clear();
					DisplayLoansForUsers(userManagementService,
						loanManagementService.GetActiveLoansByUser,
						ValidationMessages.UserHasNoBorrowedBooks);

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


	private static int LoanMenuList()
	{
		while (true)
		{
			Console.WriteLine("============================ LOAN MENU ============================");
			Console.WriteLine("1. Borrow Book");
			Console.WriteLine("2. Return Book");
			Console.WriteLine("3. Renew Loan");
			Console.WriteLine("4. View Borrowed Books");
			Console.WriteLine("5. View Loan History");
			Console.WriteLine("6. View Overdue Loans");
			Console.WriteLine("7. View User Loans");
			Console.WriteLine("8. Search Loans");
			Console.WriteLine("9. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 9) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void BorrowBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService)
	{
		var userId = ConsoleHelper.ReadInt("Enter user id", 1, int.MaxValue);
		if (userId is null) return;

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


	private static Loan? SelectActiveLoan(LoanManagementService loanManagementService, string purpose)
	{
		var userId = ConsoleHelper.ReadInt("Enter user id", 1, int.MaxValue);
		if (userId is null) return null;

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


	private static void ReturnBook(LoanManagementService loanManagementService)
	{
		var loan = SelectActiveLoan(loanManagementService, "return");
		if (loan is null) return;

		var result = loanManagementService.ReturnBook(loan.LoanId);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService)
	{
		var loan = SelectActiveLoan(loanManagementService, "renew");
		if (loan is null) return;

		var result = loanManagementService.RenewLoan(loan.LoanId);
		ConsoleHelper.ShowResult(result);
	}


	private static void DisplayLoansForUsers(UserManagementService userManagementService,
		Func<int, IReadOnlyList<Loan>> getLoansFn, string emptyMessage)
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
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}


	private static void SearchLoan(LoanManagementService loanManagementService, bool activeOnly = true)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("============================ SEARCHING LOAN MENU ============================");
			var loanList = activeOnly ? loanManagementService.GetAllActiveLoans() : loanManagementService.GetAllLoans();
			if (loanList.Count is 0)
			{
				ConsoleHelper.ShowWarning(activeOnly
					? ValidationMessages.NotAvailableActionLoan
					: ValidationMessages.NotAvailableLoan);

				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Loan ID");
			Console.WriteLine("{0, -20}", "2. Book Title");
			Console.WriteLine("{0, -20}", "3. Book ISBN");
			Console.WriteLine("{0, -20}", "4. Member Name");
			Console.WriteLine("{0, -20}", "5. Member National Code");
			Console.WriteLine("{0, -20}", "6. Status (Active/Returned)");
			Console.WriteLine("7. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 7);
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
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a book title to search",
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
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a member name to search",
						loan => $"{loan.User.FirstName} {loan.User.LastName}",
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 5:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadString(p), "Enter a member national code to search",
						loan => loan.User.NationalCode,
						(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase),
						activeOnly ? loanManagementService.SearchActiveLoans : loanManagementService.SearchLoans);

					break;
				}
				case 6:
				{
					SearchLoanAndDisplay(p => ConsoleHelper.ReadLoanStatus(p), "Enter loan status",
						loan => (LoanStatus)loan.Status, (search, value) => search == value,
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

			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
		}
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, Func<T?, Func<Loan, T?>, Func<T, T, bool>, IReadOnlyList<Loan>> searchFn)
		where T : class
	{
		var searchTerm = reader(prompt);
		if (searchTerm is null) return;

		var results = searchFn(searchTerm, selector, comparer);
		DisplayLoanResults(results);
	}


	private static void SearchLoanAndDisplay<T>(Func<string, T?> reader, string prompt, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, Func<T?, Func<Loan, T?>, Func<T, T, bool>, IReadOnlyList<Loan>> searchFn)
		where T : struct
	{
		var searchTerm = reader(prompt);
		if (!searchTerm.HasValue) return;

		var results = searchFn(searchTerm.Value, selector, comparer);
		DisplayLoanResults(results);
	}


	private static void DisplayLoanResults(IReadOnlyList<Loan> results)
	{
		if (results.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotLoanMatched);
			return;
		}

		LoanPrinter.PrintTable(results);
	}
}