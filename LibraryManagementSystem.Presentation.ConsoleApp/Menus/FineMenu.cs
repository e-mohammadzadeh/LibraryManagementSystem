using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class FineMenu
{
	public static void FineMenuController(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session, LibraryStatisticsService statisticsService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (FineMenuList())
			{
				case 1:
				{
					Console.Clear();
					ViewAllFines(fineManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewUnpaidFines(fineManagementService);
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
					PayFine(fineManagementService, userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					WaiveFine(fineManagementService, userManagementService);
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


	private static int FineMenuList()
	{
		while (true)
		{
			Console.WriteLine(new string('=', 36) + " FINE MENU " + new string('=', 36));

			Console.WriteLine("1. View All Fines");
			Console.WriteLine("2. View Unpaid Fines");
			Console.WriteLine("3. View User Fines");
			Console.WriteLine("4. Pay Fine");
			Console.WriteLine("5. Waive Fine");
			Console.WriteLine("6. Back");
			Console.WriteLine(new string('=', 82));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 6) return result;

			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
	}



	private static void ViewAllFines(IFineManagementService fineManagementService)
	{
		var fines = fineManagementService.GetAllFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUnpaidFines(IFineManagementService fineManagementService)
	{
		var fines = fineManagementService.GetAllUnpaidFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUserFines(IFineManagementService fineManagementService, UserManagementService userManagementService, ICurrentUserSession session)
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



	private static void PayFine(IFineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session)
	{
		var unpaidFines = fineManagementService.GetAllUnpaidFines();
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

		var payResult = fineManagementService.PayFine(fineId.Value);
		ConsoleHelper.ShowResult(payResult);
	}


	private static void WaiveFine(IFineManagementService fineManagementService,
		UserManagementService userManagementService)
	{
		var unpaidFines = fineManagementService.GetAllUnpaidFines();
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
}