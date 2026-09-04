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
		ICurrentUserSession session, IAuthorizationService authorization, LibraryStatisticsService statisticsService,
		LoanHistoryManagementService loanHistoryManagementService,
		FineHistoryManagementService fineHistoryManagementService, IAuditLogManagementService auditLogManagementService)
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
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (MainMenuList(authorization))
			{
				case 1:
				{
					Console.Clear();
					AuthorMenu.AuthorMenuController(authorManagementService, statisticsService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					TranslatorMenu.TranslatorMenuController(translatorManagementService, statisticsService, session,
						authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					BookMenu.BookMenuController(authorManagementService, translatorManagementService,
						bookManagementService, loanManagementService, statisticsService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					UserMenu.UserMenuController(userManagementService, statisticsService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					LoanMenu.LoanMenuController(loanManagementService, userManagementService, bookManagementService,
						statisticsService, session, authorization, loanHistoryManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					FineMenu.FineMenuController(fineManagementService, userManagementService, session,
						statisticsService, authorization, fineHistoryManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					AuditLogMenu.AuditLogMenuController(auditLogManagementService, authorization, session,
						statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowResult(authenticationService.Logout());
					return MainMenuResult.Logout;
				}
				case 9:
				{
					ConsoleHelper.ShowInfo(Messages.ExitingProgram);
					return MainMenuResult.Exit;
				}
			}
		}
	}


	private static int MainMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Authors", authorization.CanAccessAuthorManagement()),
			(2, "Translators", authorization.CanAccessTranslatorManagement()),
			(3, "Books", authorization.CanAccessBookManagement()),
			(4, "Members", authorization.CanAccessUserManagement()),
			(5, "Loans", authorization.CanAccessLoanManagement()),
			(6, "Fines", authorization.CanAccessFineManagement()),
			(7, "Audit Log", authorization.CanAccessAuditLogManagement()),
			(8, "Logout", true),
			(9, "Exit Application", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		Console.WriteLine(new string('=', 36) + " MAIN MENU " + new string('=', 36));
		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
	}
}