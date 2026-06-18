namespace LibraryManagementSystem.Presentation;

public class LoanMenu
{
	private static int LoanMenuList() {
		while (true) {
			Console.WriteLine("============================ LOAN MENU ============================");
			Console.WriteLine("1. Borrow Book");
			Console.WriteLine("2. Return Book");
			Console.WriteLine("3. Renew Loan");
			Console.WriteLine("4. View Active Loans");
			Console.WriteLine("5. View Loan History");
			Console.WriteLine("6. View Overdue Loans");
			Console.WriteLine("7. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) {
				return result;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid selection, Try again.\n");
			Console.ResetColor();
		}
	}
}