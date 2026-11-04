using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class MainMenu
{
	public static MainMenuResult MainMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, UserManagementService userManagementService,
		BookManagementService bookManagementService, LoanManagementService loanManagementService,
		IFineManagementService fineManagementService, AuthenticationService authenticationService,
		ICurrentUserSession session, IAuthorizationService authorization, LibraryStatisticsService statisticsService,
		IAuditLogManagementService auditLogManagementService, RoleManagementService roleManagementService,
		IMenuService menuService)
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
			switch (MainMenuList(menuService))
			{
				case "authors":
				{
					Console.Clear();
					AuthorMenu.AuthorMenuController(authorManagementService, statisticsService, session, authorization,
						menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "translators":
				{
					Console.Clear();
					TranslatorMenu.TranslatorMenuController(translatorManagementService, statisticsService, session,
						authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "books":
				{
					Console.Clear();
					BookMenu.BookMenuController(authorManagementService, translatorManagementService,
						bookManagementService, loanManagementService, statisticsService, session, authorization,
						menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "users":
				{
					Console.Clear();
					UserMenu.UserMenuController(userManagementService, roleManagementService, statisticsService,
						session, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "loans":
				{
					Console.Clear();
					LoanMenu.LoanMenuController(loanManagementService, menuService, userManagementService,
						bookManagementService, statisticsService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "fines":
				{
					Console.Clear();
					FineMenu.FineMenuController(fineManagementService, userManagementService, session,
						statisticsService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "auditLog":
				{
					Console.Clear();
					AuditLogMenu.AuditLogMenuController(auditLogManagementService, authorization, session,
						statisticsService);
					ConsoleHelper.Pause();
					break;
				}
				case "logout":
				{
					ConsoleHelper.ShowResult(authenticationService.Logout());
					return MainMenuResult.Logout;
				}
				case "exit":
				{
					ConsoleHelper.ShowInfo(Messages.ExitingProgram);
					return MainMenuResult.Exit;
				}
			}
		}
	}


	private static string? MainMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("main");
		return MenuHelper.DisplayMenu("MAIN MENU", items);
	}
}