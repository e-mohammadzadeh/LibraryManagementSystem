using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class MainMenu
{
	public static MainMenuResult MainMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, UserManagementService userManagementService,
		BookManagementService bookManagementService, LoanManagementService loanManagementService,
		FineManagementService fineManagementService, AuthenticationService authenticationService,
		ICurrentUserSession session, LibraryStatisticsService statisticsService)
	{
		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(ValidationMessages.SessionExpired);
				ConsoleHelper.Pause();
				return MainMenuResult.Logout;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics(), session.CurrentUser);
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
					BookMenu.BookMenuController(authorManagementService, translatorManagementService,
						bookManagementService, loanManagementService, statisticsService);
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
					Console.Clear();
					FineMenu.FineMenuController(fineManagementService, userManagementService, statisticsService);
					break;
				}
				case 7:
				{
					Console.Clear();
					ConsoleHelper.ShowError("Logged out successfully. Returning to login screen...\n");
					var result = authenticationService.Logout();
					ConsoleHelper.ShowResult(result);
					ConsoleHelper.Pause();
					return MainMenuResult.Logout;
				}
				case 8:
				{
					ConsoleHelper.ShowError("Exiting Program...\n");
					return MainMenuResult.Exit;
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
			Console.WriteLine("6. Fines");
			Console.WriteLine("7. Logout");
			Console.WriteLine("8. Exit Application");
			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 8)
			{
				return result;
			}

			ConsoleHelper.ShowError("Invalid selection, Try again.\n");
		}
	}
}