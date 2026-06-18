namespace LibraryManagementSystem.Presentation;

public class AuthorMenu
{
	private static int AuthorMenuList() {
		while (true) {
			Console.WriteLine("============================ AUTHOR MENU ============================");
			Console.WriteLine("1. Add Author");
			Console.WriteLine("2. Edit Author");
			Console.WriteLine("3. Remove Author");
			Console.WriteLine("4. Search Author");
			Console.WriteLine("5. View Author Details");
			Console.WriteLine("6. View All Authors");
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