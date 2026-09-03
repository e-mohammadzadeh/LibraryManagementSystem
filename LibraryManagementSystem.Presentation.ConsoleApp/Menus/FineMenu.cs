using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class FineMenu
{
	public static void FineMenuController(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		LibraryStatisticsService statisticsService, IAuthorizationService authorization,
		FineHistoryManagementService fineHistoryManagementService)
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
					ViewFines(fineManagementService, userManagementService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					ViewUnpaidFines(fineManagementService, userManagementService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					History(fineHistoryManagementService, userManagementService, authorization, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
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
			(4, "View Unpaid Fines", authorization.HasPermission(Permission.ViewUnpaidFines)),
			(5, "History", authorization.HasPermission(Permission.ViewFineHistory)),
			(6, "Back", true)
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
			? fineManagementService.GetUnpaidFinesByUser(session.UserId!.Value)
			: fineManagementService.GetAllUnpaidFines(session);

		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(unpaidFines);


		var fineId = ConsoleHelper.ReadInt(Messages.FineIdForPay, 1, int.MaxValue);
		if (fineId is null) return;

		var confirm = ConsoleHelper.ReadYesNo(Messages.ConfirmToPay);
		if (confirm != true) return;

		var payResult = fineManagementService.PayFine(fineId.Value, session);
		ConsoleHelper.ShowResult(payResult);
	}


	private static void WaiveFine(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var unpaidFines = fineManagementService.GetAllUnpaidFines(session);
		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(unpaidFines);

		var fineId = ConsoleHelper.ReadInt(Messages.FineIdForWaive, 1, int.MaxValue);
		if (fineId is null) return;

		var confirm = ConsoleHelper.ReadYesNo(Messages.ConfirmToWaive);
		if (confirm != true) return;

		var waiveResult = fineManagementService.WaiveFine(fineId.Value);
		ConsoleHelper.ShowResult(waiveResult);
	}


	private static void ViewFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (session.IsSelfServiceMember)
		{
			ViewOwnFines(fineManagementService, session);
			return;
		}

		if (!SessionGuard.RequirePermission(authorization, Permission.ViewFinesByUser, Messages.AccessDenied)) return;

		ViewAdministrativeFines(fineManagementService, userManagementService, authorization);
	}


	private static void ViewOwnFines(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var fines = fineManagementService.GetFinesByUser(session.UserId!.Value);
		DisplayFines(fines, Messages.FineNotFound);
	}


	private static void ViewAdministrativeFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, IAuthorizationService authorization)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " VIEW FINE MENU " + new string('=', 36));
			Console.WriteLine("1. View Fines By User");
			Console.WriteLine("2. Back");

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 2);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewFinesByUser,
						    Messages.AccessDenied))
						break;

					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
						users => UserPrinter.PrintTable(users));
					if (user is null) break;
					Console.Clear();

					var fines = fineManagementService.GetFinesByUser(user.Id);
					DisplayFines(fines, Messages.FineNotFound);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					ConsoleHelper.ShowInfo("Backing to Fine Menu");
					return;
				}
			}
		}
	}


	private static void ViewUnpaidFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (session.IsSelfServiceMember)
		{
			ViewOwnUnpaidFines(fineManagementService, session);
			return;
		}

		if (!SessionGuard.RequirePermission(authorization, Permission.ViewUnpaidFinesByUser, Messages.AccessDenied))
			return;

		ViewAdministrativeUnpaidFines(fineManagementService, userManagementService, authorization);
	}


	private static void ViewOwnUnpaidFines(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var fines = fineManagementService.GetUnpaidFinesByUser(session.UserId!.Value);
		DisplayFines(fines, Messages.UnpaidFineNotFound);
	}


	private static void ViewAdministrativeUnpaidFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, IAuthorizationService authorization)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 33) + " VIEW UNPAID FINE MENU " + new string('=', 33));
			Console.WriteLine("1. View Unpaid Fines By User");
			Console.WriteLine("2. Back");

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 2);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewUnpaidFinesByUser,
						    Messages.AccessDenied))
						break;

					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
						users => UserPrinter.PrintTable(users));
					if (user is null) break;
					Console.Clear();

					var fines = fineManagementService.GetUnpaidFinesByUser(user.Id);
					DisplayFines(fines, Messages.UnpaidFineNotFound);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					ConsoleHelper.ShowInfo("Backing to Fine Menu");
					return;
				}
			}
		}
	}


	private static void DisplayFines(IReadOnlyList<FineDto> fines, string emptyMessage)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		FinePrinter.PrintTable(fines);
	}



	private static void History(FineHistoryManagementService fineHistoryManagementService,
		UserManagementService userManagementService, IAuthorizationService authorization, ICurrentUserSession session)
	{
		if (!authorization.HasAnyPermission(Permission.ViewFineHistory, Permission.FineHistoryByUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			ConsoleHelper.Pause();
			return;
		}

		if (authorization.HasPermission(Permission.MyFullLoanHistory))
		{
			if (session.UserId is null)
			{
				ConsoleHelper.ShowError(Messages.AuthenticationRequired);
				return;
			}

			var histories = fineHistoryManagementService.GetByUserId(session.UserId!.Value);
			DisplayFineHistories(histories, Messages.NotAvailableFineHistory);
			return;
		}


		while (true)
		{
			Console.Clear();
			switch (HistoryMenuList(authorization))
			{
				case 1:
				{
					Console.Clear();
					if (!SessionGuard.RequirePermission(authorization, Permission.FineHistoryByUser,
						    Messages.AccessDenied))
						return;

					var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(),
						users => UserPrinter.PrintTable(users));
					if (user is null) return;
					var histories = fineHistoryManagementService.GetByUserId(user.Id);
					DisplayFineHistories(histories, Messages.FineNotFound);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewFineHistory,
						    Messages.AccessDenied))
						return;
					var histories = fineHistoryManagementService.GetAll();
					DisplayFineHistories(histories, Messages.FineNotFound);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					ConsoleHelper.ShowInfo("Backing to Fine Menu");
					return;
				}
			}
		}
	}


	private static int HistoryMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "History By User", authorization.HasPermission(Permission.FineHistoryByUser)),
			(2, "Full Library History", authorization.HasPermission(Permission.ViewFineHistory)),
			(3, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();
		Console.WriteLine(new string('=', 36) + " HISTORY MENU " + new string('=', 36));
		var displayNumber = 1;
		foreach (var (_, displayText, _) in availableItems) Console.WriteLine($"{displayNumber++}. {displayText}");

		Console.WriteLine(new string('=', 82));
		var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, availableItems.Count, false);
		return availableItems[choice!.Value - 1].ActionId;
	}



	private static void DisplayFineHistories(IReadOnlyList<FineHistoryDto> histories, string emptyMessage)
	{
		if (histories.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		FineHistoryPrinter.PrintTable(histories);
	}
}