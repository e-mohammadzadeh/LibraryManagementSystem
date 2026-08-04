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
					FineMenu.FineMenuController(fineManagementService, userManagementService, statisticsService, session);
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


	private static int MainMenuList(ICurrentUserSession session)
	{
		while (true)
		{
			Console.WriteLine(new string('=', 36) + " MAIN MENU " + new string('=', 36));
			if (session.CanAccessAuthorManagement) Console.WriteLine("1. Authors");

			if (session.CanAccessTranslatorManagement) Console.WriteLine("2. Translators");

			if (session.CanAccessBookManagement) Console.WriteLine("3. Books");

			if (session.CanAccessUserManagement) Console.WriteLine("4. Members");

			if (session.CanAccessLoanManagement) Console.WriteLine("5. Loans");

			if (session.CanAccessFineManagement) Console.WriteLine("6. Fines");

			Console.WriteLine("7. Logout");
			Console.WriteLine("8. Exit Application");
			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var result))
			{
				ConsoleHelper.ShowError("Invalid selection. Try again.\n");
				continue;
			}

			if (result is 7 or 8) return result;
			var isAuthorized = result switch
			{
				1 => session.CanAccessAuthorManagement,
				2 => session.CanAccessTranslatorManagement,
				3 => session.CanAccessBookManagement,
				4 => session.CanAccessUserManagement,
				5 => session.CanAccessLoanManagement,
				6 => session.CanAccessFineManagement,
				_ => false
			};
			if (isAuthorized) return result;
			ConsoleHelper.ShowError("You are not authorized to access this menu.\n");
		}
	}
}
