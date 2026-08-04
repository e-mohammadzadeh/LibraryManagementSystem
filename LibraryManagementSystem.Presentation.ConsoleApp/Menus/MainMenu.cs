using LibraryManagementSystem.Application.Authentication;
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
			switch (MainMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					AuthorMenu.AuthorMenuController(authorManagementService, statisticsService, session);
					break;
				}
				case 2:
				{
					Console.Clear();
					TranslatorMenu.TranslatorMenuController(translatorManagementService, statisticsService, session);
					break;
				}
				case 3:
				{
					Console.Clear();
					BookMenu.BookMenuController(authorManagementService, translatorManagementService,
						bookManagementService, loanManagementService, statisticsService, session);
					break;
				}
				case 4:
				{
					Console.Clear();
					UserMenu.UserMenuController(userManagementService, statisticsService, session);
					break;
				}
				case 5:
				{
					Console.Clear();
					LoanMenu.LoanMenuController(loanManagementService, userManagementService, bookManagementService,
						statisticsService, session);

					break;
				}
				case 6:
				{
					Console.Clear();
					FineMenu.FineMenuController(fineManagementService, userManagementService, session,
						statisticsService);
					break;
				}
				case 7:
				{
					Console.Clear();
					ConsoleHelper.ShowInfo(ValidationMessages.LogoutSuccess);
					var result = authenticationService.Logout();
					ConsoleHelper.ShowResult(result);
					ConsoleHelper.Pause();
					return MainMenuResult.Logout;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(ValidationMessages.ExitingProgram);
					return MainMenuResult.Exit;
				}
			}
		}
	}


	private static int MainMenuList(ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>();

		items.Add((1, "Authors", session.CanAccessAuthorManagement));
		items.Add((2, "Translators", session.CanAccessTranslatorManagement));
		items.Add((3, "Books", session.CanAccessBookManagement));
		items.Add((4, "Members", session.CanAccessUserManagement));
		items.Add((5, "Loans", session.CanAccessLoanManagement));
		items.Add((6, "Fines", session.CanAccessFineManagement));
		items.Add((7, "Logout", true));
		items.Add((8, "Exit Application", true));

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
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count)
			{
				return availableItems[userChoice - 1].ActionId;
			}
			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}
}