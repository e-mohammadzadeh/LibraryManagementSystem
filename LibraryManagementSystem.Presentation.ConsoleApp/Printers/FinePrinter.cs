using System.Reflection.PortableExecutable;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class FinePrinter
{
	public static void PrintDetails(FineDto fine)
	{
		Console.WriteLine("\nFine Details:");
		Console.WriteLine("{0, -20} [{1}]", "Fine ID:", fine.FineId);
		Console.WriteLine("{0, -20} [{1}]", "Loan ID:", fine.LoanId);
		Console.WriteLine("{0, -20} [{1}]", "User:", fine.UserFullName);
		Console.WriteLine("{0, -20} [{1}]", "Book:", fine.BookName);
		Console.WriteLine("{0, -20} [{1}]", "Overdue Days:", fine.OverdueDays);
		Console.WriteLine("{0, -20} [{1}]", "Amount:", fine.Amount);
		Console.WriteLine("{0, -20} [{1}]", "Status:", fine.Status);
		Console.WriteLine("{0, -20} [{1}]", "Reason:", fine.Reason);
		Console.WriteLine("{0, -20} [{1}]", "Created At:", fine.CreatedAt);
		Console.WriteLine("{0, -20} [{1}]", "Updated At:", fine.UpdatedAt?.ToString() ?? "-");
		Console.WriteLine("{0, -20} [{1}]", "Paid At:", fine.PaidAt?.ToString() ?? "-");
	}


	public static void PrintTable(IReadOnlyList<FineDto> fines)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.FineNotFound);
			return;
		}

		Console.WriteLine("{0,-4} {1,-8} {2,-35} {3,-35} {4,10} {5,8} {6,-10}", "ID", "Loan ID", "User", "Book",
			"Amount", "Days", "Status");
		Console.WriteLine(new string('=', 130));

		foreach (var fine in fines)
		{
			var userName = fine.UserFullName.Length > 33 ? fine.UserFullName[..20] + "..." : fine.UserFullName;
			var bookName = fine.BookName.Length > 33 ? fine.BookName[..25] + "..." : fine.BookName;

			Console.WriteLine("{0,-4} {1,-8} {2,-35} {3,-35} {4,10:C} {5,8} {6,-10}", fine.FineId, fine.LoanId,
				userName, bookName, fine.Amount, fine.OverdueDays, fine.Status);
		}
		Console.WriteLine(new string('=', 130));
	}
}