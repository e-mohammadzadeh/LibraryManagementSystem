using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;
using LibraryManagementSystem.Application.DTOs.Fine;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class FineHistoryPrinter
{
	public static void PrintTable(IReadOnlyList<FineHistoryDto> histories, string title = "Fine History")
	{
		if (histories.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableFineHistory);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		var headers = new[] { "ID", "Fine ID", "Loan ID",  "Date & Time", "Description" };
		var rows = histories.Select(history =>
		{
			var description = string.IsNullOrWhiteSpace(history.Description)
				? ["—"]
				: ConsoleTable.WrapText(history.Description, 35);

			return new[]
			{
				[history.Id.ToString()],
				[history.FineId.ToString()],
				[history.LoanId.ToString()],
				ConsoleTable.WrapText(history.BookName, 30),
				ConsoleTable.WrapText(history.UserName, 25),
				[history.OccurredAt.ToString("yyyy-MM-dd HH:mm")],
				description
			};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}