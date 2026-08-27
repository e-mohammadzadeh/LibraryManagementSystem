using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class LoanPrinter
{
	public static void PrintDetails(LoanDto loan)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("ID", [loan.LoanId.ToString()]),
			("Book Name", [loan.BookName]),
			("Book Id", [loan.BookId.ToString()]),
			("User Name", [loan.UserName]),
			("User Id", [loan.UserId.ToString()]),
			("Borrow Date", [loan.BorrowDate.ToString("yyyy-MM-dd")]),
			("Due Date", [loan.DueDate.ToString("yyyy-MM-dd")]),
			("Return Date", [loan.ReturnDate?.ToString("yyyy-MM-dd") ?? "N/A"]),
			("Status", [loan.Status.ToString()]),
			("Renewal Count", [loan.RenewalCount.ToString()]),
			("Is Overdue", [loan.IsOverdue ? "Yes" : "No"]),
			("Created At", [loan.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [loan.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Loan Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<LoanDto> loans)
	{
		if (loans.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableLoan);
			return;
		}

		var headers = new[] { "ID", "Book", "User", "Borrow Date", "Return Date", "Due Date", "Status", "Overdue" };

		var rows = loans.Select(loan => new[]
		{
			[loan.LoanId.ToString()],
			ConsoleTable.WrapText(loan.BookName, 24),
			ConsoleTable.WrapText(loan.UserName, 24),
			[loan.BorrowDate.ToString("yyyy-MM-dd")],
			[loan.ReturnDate?.ToString("yyyy-MM-dd") ?? "N/A"],
			[loan.DueDate.ToString("yyyy-MM-dd")],
			[loan.Status.ToString()],
			[loan.IsOverdue ? "Yes" : "No"]
		}).ToList();

		ConsoleTable.PrintTable("Loans List", headers, rows);
	}
}