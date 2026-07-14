using LibraryManagementSystem.Application.Services;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

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
					System.Console.Clear();
					AuthorMenu.AuthorMenuController(userManagementService);
					break;
				}
				case 2:
				{
					System.Console.Clear();
					BookMenu.BookMenuController(userManagementService, bookManagementService);
					break;
				}
				case 3:
				{
					System.Console.Clear();
					UserMenu.UserMenuController(userManagementService);
					break;
				}
				case 4:
				{
					System.Console.Clear();
					System.Console.WriteLine("Option 4 selected.");
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
			System.Console.WriteLine("============================ MAIN MENU ============================");
			System.Console.WriteLine("1. Authors");
			System.Console.WriteLine("2. Books");
			System.Console.WriteLine("3. Members");
			System.Console.WriteLine("4. Borrowing");
			System.Console.WriteLine("5. Exit");
			System.Console.WriteLine("==================================================================");
			System.Console.Write("Please Enter a number: ");

			var option = System.Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 5)
			{
				return result;
			}

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
		}
	}
}