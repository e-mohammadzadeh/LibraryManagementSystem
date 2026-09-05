using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class FinePrinter
{
	public static void PrintDetails(FineDto fine)
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("Fine ID", [fine.FineId.ToString()]),
			("Loan ID", [fine.LoanId.ToString()]),
			("User ID", [fine.UserId.ToString()]),
			("User", [fine.UserFullName]),
			("Book", [fine.BookName]),
			("Overdue Days", [fine.OverdueDays.ToString()]),
			("Amount", [$"{fine.Amount:F2}"]),
			("Status", [fine.Status.ToString()]),
			("Reason", string.IsNullOrWhiteSpace(fine.Reason) ? ["N/A"] : fine.Reason.Split('\n')),
			("Created At", [fine.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [fine.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
			("Paid At", [fine.PaidAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
		};

		ConsoleTable.PrintKeyValueTable("Fine Details", rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<FineDto> fines)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.FineNotFound);
			return;
		}

		Console.Clear();
		var headers = new[] { "ID", "Loan ID", "User", "Book", "Amount", "Days", "Status" };

		var rows = fines.Select(fine => new[]
		{
			[fine.FineId.ToString()],
			[fine.LoanId.ToString()],
			ConsoleTable.WrapText(fine.UserFullName, 28),
			ConsoleTable.WrapText(fine.BookName, 35),
			[$"{fine.Amount:F2}"],
			[fine.OverdueDays.ToString()],
			[fine.Status.ToString()]
		}).ToList();

		ConsoleTable.PrintTable("Fines List", headers, rows);
	}
}