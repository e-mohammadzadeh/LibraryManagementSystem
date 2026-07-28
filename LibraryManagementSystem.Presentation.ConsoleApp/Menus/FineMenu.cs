using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public class FineMenu
{
	private static FineManagementService _fineService = null!;


	public static void FineMenuController()
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
					ViewAllFines();
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					ViewUnpaidFines();
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					ViewUserFines();
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 4:
				{
					Console.Clear();
					PayFine();
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 5:
				{
					Console.Clear();
					WaiveFine();
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
			if (int.TryParse(option, out var result) && result is >= 1 and <= 6)
				return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}



	private static void ViewAllFines()
	{
		var fines = _fineService.GetAllFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.FineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUnpaidFines()
	{
		var fines = _fineService.GetAllUnpaidFines();
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.UnpaidFineNotFound);
			return;
		}

		FinePrinter.PrintTable(fines);
	}


	private static void ViewUserFines()
	{
		var userId = ConsoleHelper.ReadInt("Enter User ID to view fines", 1, int.MaxValue);
		if (userId is null) return;
		var fines = _fineService.GetFinesByUser(userId.Value);
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.FineNotFound);
			return;
		}
		FinePrinter.PrintTable(fines);
	}



	private static void PayFine()
	{

	}
}