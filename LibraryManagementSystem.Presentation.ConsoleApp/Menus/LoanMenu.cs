namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public class LoanMenu
{
	private static int LoanMenuList() {
		while (true) {
			System.Console.WriteLine("============================ LOAN MENU ============================");
			System.Console.WriteLine("1. Borrow Book");
			System.Console.WriteLine("2. Return Book");
			System.Console.WriteLine("3. Renew Loan");
			System.Console.WriteLine("4. View Active Loans");
			System.Console.WriteLine("5. View Loan History");
			System.Console.WriteLine("6. View Overdue Loans");
			System.Console.WriteLine("7. Back");
			System.Console.WriteLine("==================================================================");
			System.Console.Write("Please Enter a number: ");

			var option = System.Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) {
				return result;
			}
			System.Console.ForegroundColor = ConsoleColor.Red;
			System.Console.WriteLine("Invalid selection, Try again.\n");
			System.Console.ResetColor();
		}
	}
}