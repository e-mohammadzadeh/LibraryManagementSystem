using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public class MainMenu
{
	public static void MainMenuController(BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			switch (MainMenuList())
			{
				case 1:
				{
					Console.WriteLine("Option 1 selected.");
					BookMenu.BookMenuController(bookManagementService);
					break;
				}
				case 2:
				{
					Console.WriteLine("Option 2 selected.");
					break;
				}
				case 3:
				{
					Console.WriteLine("Option 3 selected.");
					break;
				}
				case 4:
				{
					Console.WriteLine("Option 4 selected.");
					break;
				}
				case 5:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Exiting Program...\n");
					Console.ResetColor();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int MainMenuList()
	{
		while (true)
		{
			Console.WriteLine("============================ MAIN MENU ============================");
			Console.WriteLine("1. Books");
			Console.WriteLine("2. Author");
			Console.WriteLine("3. Members");
			Console.WriteLine("4. Loan");
			Console.WriteLine("5. Exit");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 5)
			{
				return result;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid selection, Try again.\n");
			Console.ResetColor();
		}
	}
}