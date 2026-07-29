using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class FineMenu
{
	public static void FineMenuController(FineManagementService fineManagementService,
		UserManagementService userManagementService, LibraryStatisticsService statisticsService)
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
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewUnpaidFines(fineManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					ViewUserFines(fineManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 4:
				{
					Console.Clear();
					PayFine(fineManagementService, userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 5:
				{
					Console.Clear();
					WaiveFine(fineManagementService, userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
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
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 6) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}



	private static void ViewAllFines(FineManagementService fineManagementService)
	{
		var fines = fineManagementService.GetAllFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUnpaidFines(FineManagementService fineManagementService)
	{
		var fines = fineManagementService.GetAllUnpaidFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUserFines(FineManagementService fineManagementService)
	{
		var userId = ConsoleHelper.ReadInt("Enter User ID to view fines", 1, int.MaxValue);
		if (userId is null) return;
		var fines = fineManagementService.GetFinesByUser(userId.Value);
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}



	private static void PayFine(FineManagementService fineManagementService,
		UserManagementService userManagementService)
	{
		var unpaidFines = fineManagementService.GetAllUnpaidFines();
		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(unpaidFines);
		var fineId = ConsoleHelper.ReadInt(ValidationMessages.FineId4Pay, 1, int.MaxValue);
		if (fineId is null) return;

		var payResult = fineManagementService.PayFine(fineId.Value);
		ConsoleHelper.ShowResult(payResult);

		if (!payResult.Success || payResult.Data is null) return;
		var removeResult = userManagementService.TryAutoRemove(payResult.Data.UserId);
		if (removeResult.Success) ConsoleHelper.ShowSuccess(removeResult.Message!);
	}


	private static void WaiveFine(FineManagementService fineManagementService,
		UserManagementService userManagementService)
	{
		var unpaidFines = fineManagementService.GetAllUnpaidFines();
		if (unpaidFines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(unpaidFines);

		var fineId = ConsoleHelper.ReadInt(ValidationMessages.FineId4Waive, 1, int.MaxValue);
		if (fineId is null) return;

		var confirm = ConsoleHelper.ReadYesNo(ValidationMessages.Confirm2Waive);
		if (confirm != true) return;

		var waiveResult = fineManagementService.WaiveFine(fineId.Value);
		ConsoleHelper.ShowResult(waiveResult);

		if (!waiveResult.Success || waiveResult.Data is null) return;
		var removeResult = userManagementService.TryAutoRemove(waiveResult.Data.UserId);
		if (removeResult.Success) ConsoleHelper.ShowSuccess(removeResult.Message!);
	}
}