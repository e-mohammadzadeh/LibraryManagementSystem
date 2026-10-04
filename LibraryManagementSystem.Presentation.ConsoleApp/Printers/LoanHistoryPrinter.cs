using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class LoanHistoryPrinter
{
	public static void PrintTable(IReadOnlyList<LoanHistoryDto> histories, string title = "Loan History")
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
			var description = string.IsNullOrWhiteSpace(history.Description)
				? ["—"]
				: ConsoleTable.WrapText(history.Description, 35);

			return new[]
			{
				[(index + 1).ToString()],
				[history.LoanId.ToString()[..8]],
				ConsoleTable.WrapText(history.BookName, 30),
				ConsoleTable.WrapText(history.UserName, 25),
				[history.OccurredAt.ToString("yyyy-MM-dd HH:mm")],
				description
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}