using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class AuditLogMenu
{
	public static void AuditLogMenuController(IAuditLogManagementService auditLogManagementService,
		IMenuService menuService, IAuthorizationService authorization, ICurrentUserSession session,
		LibraryStatisticsService statisticsService)
	{
		if (!authorization.HasPermission(Permission.ViewAuditLog)) return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (AuditLogMenuList(menuService))
			{
				case "audit-log.view":
				{
					if (!authorization.HasPermission(Permission.ViewAuditLog))
						break;
					Console.Clear();
					var auditLogs = auditLogManagementService.GetAll();
					AuditLogPrinter.PrintTable(auditLogs);
					ConsoleHelper.Pause();
					break;
				}
				case "audit-log.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? AuditLogMenuList(IMenuService menuService)
	{
		return MenuHelper.DisplayMenu("AUDIT LOG MENU", menuService.GetChildren("audit-log"));
	}
}