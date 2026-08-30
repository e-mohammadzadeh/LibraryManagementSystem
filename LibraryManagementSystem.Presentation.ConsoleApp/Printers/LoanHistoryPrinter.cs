using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class LoanHistoryPrinter
{
	public static void PrintTable(
		IReadOnlyList<LoanHistoryDto> histories,
		string title = "Loan History")
	{
		if (histories.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableLoanHistory);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		var headers = new[] { "ID", "Loan ID", "Book", "User", "Action", "Date & Time", "Description" };
		var rows = histories.Select(history =>
		{
			var description = string.IsNullOrWhiteSpace(history.Description)
				? ["—"]
				: ConsoleTable.WrapText(history.Description, 35);

			return new[]
			{
				[history.Id.ToString()],
				[history.LoanId.ToString()],
				[history.BookName],
				[history.UserName],
				[history.Action.ToString()],
				[history.OccurredAt.ToString("yyyy-MM-dd HH:mm")],
				description
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}