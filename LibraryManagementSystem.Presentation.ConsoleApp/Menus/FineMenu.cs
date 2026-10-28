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
		LibraryStatisticsService statisticsService, IAuthorizationService authorization)
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
			switch (FineMenuList(authorization))
			{
				case 1:
				{
					Console.Clear();
					PayFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.WaiveFine, Messages.AccessDenied))
						break;
					Console.Clear();
					WaiveFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					ViewFines(fineManagementService, userManagementService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static int FineMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Pay Fine", true),
			(2, "Waive Fine", authorization.HasPermission(Permission.WaiveFine)),
			(3, "View Fines", true),
			(4, "Back", true)
		};

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
		UserManagementService userManagementService, ICurrentUserSession session,
		IAuthorizationService authorization)
	{
		if (!session.IsAuthenticated)
		{
			ConsoleHelper.ShowError(Messages.SessionExpired);
			return;
		}

		while (true)
		{
			Console.Clear();
			switch (ShowViewFineMenu(authorization))
			{
				case 1:
				{
					Console.Clear();
					BrowseMyFines(fineManagementService, session, authorization);
					break;
				}
				case 2:
				{
					Console.Clear();
					BrowseFinesByUser(fineManagementService, userManagementService, authorization);
					break;
				}
				case 3:
				{
					Console.Clear();
					BrowseFinesFullLibrary(fineManagementService, session, authorization);
					break;
				}
				case 4:
				{
					ConsoleHelper.ShowInfo(Messages.BackToFineMenu);
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static int ShowViewFineMenu(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string Text, bool Available)>
		{
			(1, "My fines", true),
			(2, "By user", authorization.HasPermission(Permission.ViewFinesByUser)),
			(3, "Full library", authorization.HasPermission(Permission.ViewAllFines)),
			(4, "Back", true)
		};

		return MenuHelper.DisplayMenu("VIEW FINES", items);
	}


	private static FineFilter? PickFineFilter(IAuthorizationService authorization, Permission? filterPermission,
		bool allowAll)
	{
		var canView = filterPermission is null || authorization.HasPermission(filterPermission.Value);
		var items = new List<(FineFilter? Filter, string Text, bool Available)>
		{
			(FineFilter.Unpaid, "Unpaid", canView),
			(FineFilter.Paid, "Paid", canView),
			(FineFilter.All, "All (history)", allowAll && authorization.HasPermission(Permission.ViewAllFines)),
			(null, "Back", true)
		};

		return MenuHelper.DisplayMenu("FINE STATUS", items);
	}


	private static void BrowseMyFines(IFineManagementService fineManagementService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		var filter = PickFineFilter(authorization, filterPermission: null, allowAll: false);
		if (filter is null) return;

		Console.Clear();
		DisplayFines(fineManagementService.GetFinesByUser(session.CurrentUser.Id, filter.Value),
			filter.Value == FineFilter.Unpaid ? Messages.UnpaidFineNotFound : Messages.FineNotFound);
	}


	private static void BrowseFinesByUser(IFineManagementService fineManagementService,
		UserManagementService userManagementService, IAuthorizationService authorization)
	{
		var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
			users => UserPrinter.PrintTable(users));
		if (user is null) return;

		var filter = PickFineFilter(authorization, Permission.ViewFinesByUser, allowAll: true);
		if (filter is null) return;

		Console.Clear();
		DisplayFines(fineManagementService.GetFinesByUser(user.Id, filter.Value), Messages.FineNotFound);
	}


	private static void BrowseFinesFullLibrary(IFineManagementService fineManagementService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		var filter = PickFineFilter(authorization, Permission.ViewAllFines, allowAll: true);
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