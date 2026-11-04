using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class FineMenu
{
	public static void FineMenuController(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		LibraryStatisticsService statisticsService, IAuthorizationService authorization, IMenuService menuService)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.ViewFine,
			    Permission.WaiveFine,
			    Permission.ViewFinesByUser,
			    Permission.ViewAllFines))
			return;

		while (true)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (FineMenuList(menuService))
			{
				case "fines.pay":
				{
					Console.Clear();
					PayFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "fines.waive":
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.WaiveFine, Messages.AccessDenied))
						break;
					Console.Clear();
					WaiveFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "fines.view":
				{
					Console.Clear();
					ViewFines(fineManagementService, userManagementService, session, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "fines.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? FineMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("fines");
		return MenuHelper.DisplayMenu("FINE MENU", items);
	}


	private static void PayFine(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var unpaidFines = session.IsSelfServiceMember
			? fineManagementService.GetFinesByUser(session.CurrentUser.Id, FineFilter.Unpaid)
			: fineManagementService.GetAll(session, FineFilter.Unpaid);

		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		Console.Clear();
		FinePrinter.PrintTable(unpaidFines);

		var selection = ConsoleHelper.ReadInt(Messages.FineIdForPay, 1, unpaidFines.Count);
		if (selection is null) return;

		var fineToPay = unpaidFines[selection.Value - 1];

		var confirm = ConsoleHelper.ReadYesNo(Messages.ConfirmToPay);
		if (confirm != true) return;

		var payResult = fineManagementService.PayFine(fineToPay.Id, session);
		ConsoleHelper.ShowResult(payResult);
	}


	private static void WaiveFine(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var unpaidFines = fineManagementService.GetAll(session, FineFilter.Unpaid);

		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		Console.Clear();
		FinePrinter.PrintTable(unpaidFines);

		var selection = ConsoleHelper.ReadInt(Messages.FineIdForWaive, 1, unpaidFines.Count);
		if (selection is null) return;

		var fineToWaive = unpaidFines[selection.Value - 1];

		var confirm = ConsoleHelper.ReadYesNo(Messages.ConfirmToWaive);
		if (confirm != true) return;

		var waiveResult = fineManagementService.WaiveFine(fineToWaive.Id);
		ConsoleHelper.ShowResult(waiveResult);
	}


	private static void ViewFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, IMenuService menuService)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError(Messages.SessionExpired);
			return;
		}

		while (true)
		{
			Console.Clear();
			switch (ShowViewFineMenu(menuService))
			{
				case "fines.view.own":
				{
					Console.Clear();
					BrowseMyFines(fineManagementService, session, menuService);
					break;
				}
				case "fines.view.by-user":
				{
					Console.Clear();
					BrowseFinesByUser(fineManagementService, userManagementService, menuService);
					break;
				}
				case "fines.view.full":
				{
					Console.Clear();
					BrowseFinesFullLibrary(fineManagementService, session, menuService);
					break;
				}
				case "fines.view.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToFineMenu);
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static string? ShowViewFineMenu(IMenuService menuService)
	{
		var items = menuService.GetChildren("fines.view");
		return MenuHelper.DisplayMenu("VIEW FINES MENU", items);
	}


	private static FineFilter? PickFineFilter(IMenuService menuService)
	{
		var selectedItem = MenuHelper.DisplayMenu("FINE FILTER", menuService.GetChildren("fines.filter"));
		return selectedItem switch
		{
			"fines.filter.unpaid" => FineFilter.Unpaid,
			"fines.filter.paid" => FineFilter.Paid,
			"fines.filter.waived" => FineFilter.Waived,
			"fines.filter.all" => FineFilter.All, 
			_ => null
		};
	}


	private static void BrowseMyFines(IFineManagementService fineManagementService,
		ICurrentUserSession session, IMenuService menuService)
	{
		var filter = PickFineFilter(menuService);
		if (filter is null) return;

		Console.Clear();
		DisplayFines(fineManagementService.GetFinesByUser(session.CurrentUser.Id, filter.Value),
			filter.Value == FineFilter.Unpaid ? Messages.UnpaidFineNotFound : Messages.FineNotFound);
	}


	private static void BrowseFinesByUser(IFineManagementService fineManagementService,
		UserManagementService userManagementService, IMenuService menuService)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
			users => UserPrinter.PrintTable(users));
		if (user is null) return;

		var filter = PickFineFilter(menuService);
		if (filter is null) return;

		Console.Clear();
		DisplayFines(fineManagementService.GetFinesByUser(user.Id, filter.Value), Messages.FineNotFound);
	}


	private static void BrowseFinesFullLibrary(IFineManagementService fineManagementService,
		ICurrentUserSession session, IMenuService menuService)
	{
		var filter = PickFineFilter(menuService);
		if (filter is null) return;

		Console.Clear();
		DisplayFines(fineManagementService.GetAll(session, filter.Value), Messages.FineNotFound);
	}


	private static void DisplayFines(IReadOnlyList<FineDto> fines, string emptyMessage)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		Console.Clear();
		FinePrinter.PrintTable(fines);
	}
}