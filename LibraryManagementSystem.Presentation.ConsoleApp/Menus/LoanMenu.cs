using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;
using System.Drawing;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public class LoanMenu
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
					break;
				}
				case 2:
				{
					Console.Clear();
					ReturnBook();
					break;
				}
				case 3:
				{
					Console.Clear();

					break;
				}
				case 4:
				{
					Console.Clear();

					break;
				}
				case 5:
				{
					Console.Clear();

					break;
				}
				case 6:
				{
					Console.Clear();

					break;
				}
				case 7:
				{
					Console.Clear();

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
		int userIntValue, bookIntValue;
		while (true)
		{
			Console.Write("Enter user id: ");
			var userId = Console.ReadLine();
			if (!int.TryParse(userId, out var userValue))
			{
				Console.WriteLine("wrong input, please enter user's id.");
				continue;
			}

			userIntValue = userValue;
			break;
		}

		while (true)
		{
			if (bookManagementService.GetAllBooks().Count is 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
				return;
			}

			BookPrinter.PrintTable(bookManagementService.GetAllBooks());
			Console.Write("Enter your desired book id to borrow: ");
			var bookId = Console.ReadLine();
			if (!int.TryParse(bookId, out var bookValue))
			{
				Console.WriteLine("wrong input, please enter book's id.");
				continue;
			}

			bookIntValue = bookValue;
			break;
		}

		var result = loanManagementService.BorrowBook(userIntValue, bookIntValue);
		ConsoleHelper.ShowResult(result);
	}


	private static void ReturnBook(LoanManagementService loanManagementService,
		BookManagementService bookManagementService)
	{
		int userIntValue, bookIntValue;
		while (true)
		{
			Console.Write("Enter user id: ");
			var userId = Console.ReadLine();
			if (!int.TryParse(userId, out var userValue))
			{
				Console.WriteLine("wrong input, please enter user's id.");
				continue;
			}

			userIntValue = userValue;
			break;
		}

		while (true)
		{
			if (bookManagementService.GetAllBooks().Count is 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
				return;
			}

			var temp = ILoanRepository.GetActiveLoansByUser(userIntValue);
			BookPrinter.PrintTable(Print borrowed books);
			Console.Write("Enter your desired book id to return: ");
			var bookId = Console.ReadLine();
			if (!int.TryParse(bookId, out var bookValue))
			{
				Console.WriteLine("wrong input, please enter book's id.");
				continue;
			}

			bookIntValue = bookValue;
			break;
		}

		var result = loanManagementService.ReturnBook(userIntValue, bookIntValue);
		ConsoleHelper.ShowResult(result);
	}
}