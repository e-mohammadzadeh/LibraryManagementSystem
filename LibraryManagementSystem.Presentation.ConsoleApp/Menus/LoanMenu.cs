using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoanMenu
{
	public static void LoanMenuController(LoanManagementService loanManagementService,
		UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
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
					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers());
					if (user is null)
					{
						ConsoleHelper.ShowWarning(ValidationMessages.UserNotFound);
						break;
					}

					var loans = loanManagementService.GetLoansByUser(user.Id);
					LoanPrinter.PrintTable(loans);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
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

					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 8:
				{
					Console.Clear();

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


		if (bookManagementService.GetAllBooks().Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
			return;
		}

		BookPrinter.PrintTable(bookManagementService.GetAllBooks());
		var bookId = ConsoleHelper.ReadInt("Enter your desired book id to borrow: ", 1, int.MaxValue);
		if (bookId is null) return;

		var result = loanManagementService.BorrowBook((int)userId, (int)bookId);
		ConsoleHelper.ShowResult(result);
	}


	private static void ReturnBook(LoanManagementService loanManagementService)
	{
		var userId = ConsoleHelper.ReadInt("Enter user id", 1, int.MaxValue);
		if (userId is null) return;

		var loans = loanManagementService.GetActiveLoansByUser((int)userId);
		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UserHasNoBorrowedBooks);
			return;
		}

		LoanPrinter.PrintTable(loans);
		var bookId = ConsoleHelper.ReadInt("Enter your desired book id to return: ", 1, int.MaxValue);
		if (bookId is null) return;
		if (loans.All(l => l.BookId != bookId))
		{
			ConsoleHelper.ShowError("This book is not borrowed by this user.");
			return;
		}

		var result = loanManagementService.ReturnBook((int)userId, (int)bookId);
		ConsoleHelper.ShowResult(result);
	}


	private static void RenewLoan(LoanManagementService loanManagementService)
	{
		var userId = ConsoleHelper.ReadInt("Enter user id", 1, int.MaxValue);
		if (userId is null)
			return;

		var loans = loanManagementService.GetActiveLoansByUser((int)userId);
		if (loans.Count is 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UserHasNoBorrowedBooks);
			return;
		}

		LoanPrinter.PrintTable(loans);
		var bookId = ConsoleHelper.ReadInt("Enter your desired book id to return: ", 1, int.MaxValue);
		if (bookId is null)
			return;

		if (loans.All(l => l.BookId != bookId))
		{
			ConsoleHelper.ShowError("This book is not borrowed by this user.");
			return;
		}

		var result = loanManagementService.RenewLoan((int)userId, (int)bookId);
		ConsoleHelper.ShowResult(result);
	}
}