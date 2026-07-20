using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class MainMenu
{
	public static void MainMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, UserManagementService userManagementService,
		BookManagementService bookManagementService, LoanManagementService loanManagementService,
		LibraryStatisticsService statisticsService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (MainMenuList())
			{
				case 1:
				{
					Console.Clear();
					AuthorMenu.AuthorMenuController(authorManagementService, statisticsService);
					break;
				}
				case 2:
				{
					Console.Clear();
					TranslatorMenu.TranslatorMenuController(translatorManagementService, statisticsService);
				}
				case 3:
				{
					Console.Clear();
					BookMenu.BookMenuController(authorManagementService, bookManagementService, statisticsService);
					break;
				}
				case 4:
				{
					Console.Clear();
					UserMenu.UserMenuController(userManagementService, statisticsService);
					break;
				}
				case 5:
				{
					Console.Clear();
					LoanMenu.LoanMenuController(loanManagementService, userManagementService, bookManagementService,
						statisticsService);

					break;
				}
				case 6:
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
			Console.WriteLine("4. Loans");
			Console.WriteLine("5. Exit");
			Console.WriteLine("===================================================================");
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