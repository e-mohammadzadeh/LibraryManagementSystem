using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;
using System.Diagnostics;

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
			    Permission.PayFine,
			    Permission.WaiveFine,
			    Permission.ViewFines,
			    Permission.ViewFinesByUser,
			    Permission.ViewUnpaidFines,
			    Permission.ViewUnpaidFinesByUser,
			    Permission.ViewFineHistory,
			    Permission.FineHistoryByUser))
		{
			return;
		}

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
					if (!SessionGuard.RequirePermission(authorization, Permission.PayFine, Messages.AccessDenied))
						break;
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
					BrowseFines(fineManagementService, userManagementService, session, authorization);
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
			(1, "Pay Fine", authorization.HasPermission(Permission.PayFine)),
			(2, "Waive Fine", authorization.HasPermission(Permission.WaiveFine)),
			(3, "View Fines", authorization.HasPermission(Permission.ViewFines)),
			(4, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 36) + " FINE MENU " + new string('=', 36));
		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
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

		FinePrinter.PrintTable(unpaidFines);

		var selection = ConsoleHelper.ReadInt(Messages.FineIdForWaive, 1, unpaidFines.Count);
		if (selection is null) return;

		var fineToWaive = unpaidFines[selection.Value - 1];

		var confirm = ConsoleHelper.ReadYesNo(Messages.ConfirmToWaive);
		if (confirm != true) return;

		var waiveResult = fineManagementService.WaiveFine(fineToWaive.Id);
		ConsoleHelper.ShowResult(waiveResult);
	}


	private static void BrowseFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		IAuthorizationService authorization)
	{
		// ---------- Axis 1: scope (gated by permission) ----------
		var canByUser = authorization.HasPermission(Permission.ViewFinesByUser);
		var canFullLib = authorization.HasPermission(Permission.ViewFineHistory); // see note below

		var scopeItems = new List<(int Id, string Text)>
		{
			(1, "My fines")
		};
		if (canByUser) scopeItems.Add((2, "By user"));
		if (canFullLib) scopeItems.Add((3, "Full library"));

		int scope;
		if (scopeItems.Count == 1)
			scope = 1;
		else
		{
			Console.Clear();
			Console.WriteLine(new string('=', 40) + " FINES " + new string('=', 40));
			for (var i = 0; i < scopeItems.Count; i++) Console.WriteLine($"{i + 1}. {scopeItems[i].Text}");

			var scopeChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, scopeItems.Count);
			if (scopeChoice is null) return;
			scope = scopeItems[scopeChoice.Value - 1].Id;
		}

		Guid? targetUserId = null;
		if (scope == 2)
		{
			var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
				users => UserPrinter.PrintTable(users));
			if (user is null) return;
			targetUserId = user.Id;
		}

		// ---------- Axis 2: status filter ("All" = Admin only) ----------
		var canSeeAllStatuses = authorization.HasPermission(Permission.ViewFineHistory); // Admin-held

		var filterItems = new List<(FineFilter Filter, string Text)>
		{
			(FineFilter.Unpaid, "Unpaid"),
			(FineFilter.Paid, "Paid"),
		};
		if (canSeeAllStatuses) filterItems.Add((FineFilter.All, "All (history)"));

		Console.Clear();
		Console.WriteLine(new string('=', 38) + " STATUS " + new string('=', 38));
		for (var i = 0; i < filterItems.Count; i++) Console.WriteLine($"{i + 1}. {filterItems[i].Text}");

		var filterChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, filterItems.Count);
		if (filterChoice is null) return;
		var filter = filterItems[filterChoice.Value - 1].Filter;

		// ---------- Fetch & display ----------
		Console.Clear();
		var fines = scope switch
		{
			1 => fineManagementService.GetFinesByUser(session.CurrentUser.Id, filter),
			2 => fineManagementService.GetFinesByUser(targetUserId!.Value, filter),
			3 => fineManagementService.GetAll(session, filter),
			_ => throw new UnreachableException()
		};

		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.FineNotFound);
			ConsoleHelper.Pause();
			return;
		}

		FinePrinter.PrintTable(fines);
	}
}