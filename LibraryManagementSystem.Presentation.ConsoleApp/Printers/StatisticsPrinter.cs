using LibraryManagementSystem.Application.DTOs.Library;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public static class StatisticsPrinter
{
	public static void Print(LibraryStatisticsDto statistics)
	{
		Console.WriteLine("========================= LIBRARY DASHBOARD =========================");
		Console.WriteLine($"Books        : {statistics.TotalBooks}");
		Console.WriteLine($"Authors      : {statistics.TotalAuthors}");
		Console.WriteLine($"Users        : {statistics.TotalUsers}");
		Console.WriteLine($"Active Loans : {statistics.TotalActiveLoans}\n");
		//Console.WriteLine("==================================================");
	}
}