using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public static class MainMenu
{
	public static void MainMenuController(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			switch (MainMenuList())
			{
				case 1:
				{
					Console.Clear();
					AuthorMenu.AuthorMenuController(userManagementService);
					break;
				}
				case 2:
				{
					Console.Clear();
					BookMenu.BookMenuController(userManagementService, bookManagementService);
					break;
				}
				case 3:
				{
					Console.Clear();
					MemberMenu.MemberMenuController(userManagementService);
					break;
				}
				case 4:
				{
					Console.Clear();
					Console.WriteLine("Option 4 selected.");
					break;
				}
				case 5:
				{
					ConsoleHelper.ShowError("Exiting Program...\n");
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
			Console.WriteLine("1. Authors");
			Console.WriteLine("2. Books");
			Console.WriteLine("3. Members");
			Console.WriteLine("4. Borrowing");
			Console.WriteLine("5. Exit");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 5)
			{
				return result;
			}

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
		}
	}
}