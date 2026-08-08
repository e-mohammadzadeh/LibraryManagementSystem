using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class MainMenu
{
	public static MainMenuResult MainMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, UserManagementService userManagementService,
		BookManagementService bookManagementService, LoanManagementService loanManagementService,
		IFineManagementService fineManagementService, AuthenticationService authenticationService,
		ICurrentUserSession session, IAuthorizationService authorization ,LibraryStatisticsService statisticsService)
	{
		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return MainMenuResult.Logout;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			switch (MainMenuList())
			{
				case 1:
				{
					AuthorMenu.AuthorMenuController(authorManagementService, statisticsService, session, authorization);
					break;
				}
				case 2:
				{
					TranslatorMenu.TranslatorMenuController(translatorManagementService, statisticsService, session, authorization);
					break;
				}
				case 3:
				{
					BookMenu.BookMenuController(authorManagementService, translatorManagementService,
						bookManagementService, loanManagementService, statisticsService, session, authorization);
					break;
				}
				case 4:
				{
					Console.Clear();
					UserMenu.UserMenuController(userManagementService, statisticsService, session, authorization);
					break;
				}
				case 5:
				{
					LoanMenu.LoanMenuController(loanManagementService, userManagementService, bookManagementService,
						statisticsService, session, authorization);

					break;
				}
				case 6:
				{
					FineMenu.FineMenuController(fineManagementService, userManagementService, session,
						statisticsService, authorization);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(Messages.LogoutSuccess);
					var result = authenticationService.Logout();
					ConsoleHelper.ShowResult(result);
					return MainMenuResult.Logout;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(Messages.ExitingProgram);
					return MainMenuResult.Exit;
				}
			}
			ConsoleHelper.Pause();
		}
	}


	private static int MainMenuList()
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Authors", true),
			(2, "Translators", true),
			(3, "Books", true),
			(4, "Members", true),
			(5, "Loans", true),
			(6, "Fines", true),
			(7, "Logout", true),
			(8, "Exit Application", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();



		while (true)
		{
			Console.WriteLine(new string('=', 36) + " MAIN MENU " + new string('=', 36));
			var displayNumber = 1;
			foreach (var item in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {item.DisplayText}");
				displayNumber++;
			}
			Console.WriteLine(new string('=', 82));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count)
			{
				return availableItems[userChoice - 1].ActionId;
			}
			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
	}
}