using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class LoanPrinter
{
	public static void PrintTable(IReadOnlyList<Loan> loans)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableLoan);
			return;
		}

		Console.WriteLine("{0,3} {1, 30} {2, 30} {3, 12} {4, 12} {5, 12}", "ID", "Book", "User", "Borrow Date",
			"Due Date", "Status");

		Console.WriteLine(
			"===========================================================================================================");

		foreach (var loan in loans)
		{
			var userName = loan.User.FirstName + " " + loan.User.LastName;
			Console.WriteLine("{0,3} {1, 30} {2, 30} {3, 12} {4, 12} {5, 12}", loan.LoanId, loan.Book.BookName,
				userName, loan.BorrowDate, loan.DueDate, loan.Status);
		}

		Console.WriteLine(
			"===========================================================================================================");
	}
}