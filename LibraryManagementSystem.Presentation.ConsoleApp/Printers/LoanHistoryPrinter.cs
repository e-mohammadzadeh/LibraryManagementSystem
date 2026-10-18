using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class LoanHistoryPrinter
{
	public static void PrintTable(IReadOnlyList<LoanDto> histories, string title = "Loan History")
	{
		if (histories.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableLoanHistory);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		var headers = new[] { "#", "Loan ID", "Book", "User", "Date & Time", "Description" };
		var rows = histories.Select((history, index) =>
		{
			return new[]
			{
				[(index + 1).ToString()],
				[history.Id.ToString()[..8]],
				ConsoleTable.WrapText(history.BookName, 30),
				ConsoleTable.WrapText(history.UserName, 25),
				[history.CreatedAt.ToString("yyyy-MM-dd HH:mm")]
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}