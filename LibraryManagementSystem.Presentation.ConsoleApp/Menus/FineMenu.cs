using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class FineMenu
{
	public static void FineMenuController(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		LibraryStatisticsService statisticsService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			switch (FineMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					ViewAllFines(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewUnpaidFines(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					ViewUserFines(fineManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					PayFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					if (!SessionGuard.RequireAdmin(session)) break;
					WaiveFine(fineManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					ConsoleHelper.Pause();
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int FineMenuList(ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "View All Fines", session.IsAdmin || session.IsLibrarian),
			(2, "View Unpaid Fines", true),
			(3, "View User Fines", true),
			(4, "Pay Fine", true),
			(5, "Waive Fine", session.IsAdmin),
			(6, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " FINE MENU " + new string('=', 36));

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

			if (userChoice >= 1 && userChoice <= availableItems.Count) return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
	}



	private static void ViewAllFines(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var fines = session.IsSelfServiceMember
			? fineManagementService.GetFinesByUser(session.UserId!.Value)
			: fineManagementService.GetAllFines(session);
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUnpaidFines(IFineManagementService fineManagementService, ICurrentUserSession session)
	{
		var fines = session.IsSelfServiceMember
			? fineManagementService.GetUnpaidFinesByUser(session.UserId!.Value)
			: fineManagementService.GetAllUnpaidFines(session);
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUserFines(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var desiredUser = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
		if (desiredUser is null) return;
		var fines = fineManagementService.GetFinesByUser(desiredUser.Id);
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
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

		if (session.IsSelfServiceMember)
		{
			var selected = unpaidFines.FirstOrDefault(f => f.FineId == fineId.Value);
			if (selected is null || selected.UserId != session.UserId)
			{
				ConsoleHelper.ShowError(Messages.CanPayOwnFine);
				return;
			}
		}

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

		var waiveResult = fineManagementService.WaiveFine(fineId.Value, session);
		ConsoleHelper.ShowResult(waiveResult);
	}
}