using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using System.Text;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class FineHistoryPrinter
{
	public static void PrintTable(IReadOnlyList<FineDto> histories, string title = "Fine History")
	{
		if (histories.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableFineHistory);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

		var headers = new[] { "#", "Fine ID", "Loan ID", "User Name", "Date & Time", "Amount", "Description" };
		var rows = histories.Select((history, index) =>
		{
			return new[]
			{
				[(index + 1).ToString()],
				[history.Id.ToString()],
				[history.LoanId.ToString()],
				ConsoleTable.WrapText(history.UserFullName, 25),
				[history.CreatedAt.ToString("yyyy-MM-dd HH:mm")],
				[history.Money.ToString()]};
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}