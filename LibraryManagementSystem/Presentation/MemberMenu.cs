namespace LibraryManagementSystem.Presentation;

public class MemberMenu
{
	private static int MemberMenuList() {
		while (true) {
			Console.WriteLine("============================ MEMBER MENU ============================");
			Console.WriteLine("1. Register Member");
			Console.WriteLine("2. Edit Member");
			Console.WriteLine("3. Remove Member");
			Console.WriteLine("4. Search Member");
			Console.WriteLine("5. View Member Details");
			Console.WriteLine("6. View All Members");
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