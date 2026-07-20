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
					break;
				}
				case 3:
				{
					Console.Clear();
					BookMenu.BookMenuController(authorManagementService, translatorManagementService, bookManagementService, statisticsService);
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
			Console.WriteLine(new string('=', 36) + " MAIN MENU " + new string('=', 36));
			Console.WriteLine("1. Authors");
			Console.WriteLine("2. Translators");
			Console.WriteLine("3. Books");
			Console.WriteLine("4. Members");
			Console.WriteLine("5. Loans");
			Console.WriteLine("6. Exit");
			Console.WriteLine(new string('=', 82));
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 6)
			{
				return result;
			}

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
		}
	}
}