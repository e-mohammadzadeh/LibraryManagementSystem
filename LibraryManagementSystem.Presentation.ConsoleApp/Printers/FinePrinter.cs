using System.Text;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class FinePrinter
{
	public static void PrintDetails(FineDto fine)
	{
		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

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


	//public static void PrintTable(IReadOnlyList<FineDto> fines)
	//{
	//	if (fines.Count == 0)
	//	{
	//		ConsoleHelper.ShowError(Messages.FineNotFound);
	//		return;
	//	}

	//	Console.Clear();
	//	Console.WriteLine("\n{0,-4} {1,-8} {2,-35} {3,-35} {4,-10} {5,-8} {6,-10}", "ID", "Loan ID", "User", "Book",
	//		"Amount", "Days", "Status");
	//	Console.WriteLine(new string('=', 130));

	//	foreach (var fine in fines)
	//	{
	//		var userName = fine.UserFullName.Length > 33 ? fine.UserFullName[..20] + "..." : fine.UserFullName;
	//		var bookName = fine.BookName.Length > 33 ? fine.BookName[..25] + "..." : fine.BookName;

	//		Console.WriteLine("{0,-4} {1,-8} {2,-35} {3,-35} {4,-10} {5,-8} {6,-10}", fine.FineId, fine.LoanId,
	//			userName, bookName, $"{fine.Amount:F2}", fine.OverdueDays, fine.Status);
	//	}

	//	Console.WriteLine(new string('=', 130));
	//}


	public static void PrintTable(IReadOnlyList<FineDto> fines)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.FineNotFound);
			return;
		}

		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

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