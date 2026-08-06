using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class LoanPrinter
{
	public static void PrintDetails(LoanDto loan) 
	{
		Console.WriteLine("\nLoan Details:");
		Console.WriteLine("{0, -20} [{1}]", "ID:", loan.LoanId);
		Console.WriteLine("{0, -20} [{1}]", "Book Name:", loan.BookName);
		Console.WriteLine("{0, -20} [{1}]", "Book Id:", loan.BookId);
		Console.WriteLine("{0, -20} [{1}]", "User Name:", loan.UserName);
		Console.WriteLine("{0, -20} [{1}]", "User Id:", loan.UserId);
		Console.WriteLine("{0, -20} [{1}]", "Borrow Date:", loan.BorrowDate);
		Console.WriteLine("{0, -20} [{1}]", "Due Date:", loan.DueDate);
		Console.WriteLine("{0, -20} [{1}]", "Return Date:", loan.ReturnDate);
		Console.WriteLine("{0, -20} [{1}]", "Status:", loan.Status);
		Console.WriteLine("{0, -20} [{1}]", "Renewal Count:", loan.RenewalCount);
		Console.WriteLine("{0, -20} [{1}]", "Is Overdue:", loan.IsOverdue);
		Console.WriteLine("{0, -20} [{1}]", "Created At:", loan.CreatedAt);
		Console.WriteLine("{0, -20} [{1}]", "Updated At:", loan.UpdatedAt);
	}



	public static void PrintTable(IReadOnlyList<LoanDto> loans)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableLoan);
			return;
		}

		Console.WriteLine("{0,-3} {1, -50} {2, -30} {3, -12} {4, -12} {5, -12} {6, -12} {7, -12}", "ID", "Book", "User",
			"Borrow Date", "Return Date", "Due Date", "Status", "Overdue");

		Console.WriteLine(new string('=', 170));

		foreach (var loan in loans)
		{
			Console.WriteLine("{0,-3} {1, -50} {2, -30} {3, -12} {4, -12} {5, -12} {6, -12} {7, -12}", loan.LoanId,
				loan.BookName, loan.UserName, loan.BorrowDate, loan.ReturnDate, loan.DueDate, loan.Status, loan.IsOverdue? "\u2705": "\u274C");
		}

		Console.WriteLine(new string('=', 170));
	}
}