using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
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
			    Permission.PayFine,
			    Permission.WaiveFine,
			    Permission.ViewFines,
			    Permission.MyFines,
			    Permission.ViewFinesByUser,
			    Permission.ViewFinesByBook,
			    Permission.ViewUnpaidFines,
			    Permission.MyUnpaidFines,
			    Permission.ViewUnpaidFinesByUser,
			    Permission.ViewUnpaidFinesByBook,
			    Permission.ViewFineHistory,
			    Permission.FineHistoryByUser,
			    Permission.FineHistoryByBook))
		{
			return;
		}

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
			switch (FineMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.PayFine, Messages.AccessDenied))
						break;
					PayFine(fineManagementService, session);
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.WaiveFine, Messages.AccessDenied))
						break;
					WaiveFine(fineManagementService, session);
					break;
				}
				case 3:
				{
					ViewAllFines(fineManagementService, session, authorization);
					break;
				}
				case 4:
				{
					ViewUnpaidFines(fineManagementService, session, authorization);
					break;
				}
				case 5:
				{
					History(fineManagementService, session, authorization);
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}

			ConsoleHelper.Pause();
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


	private static void ViewFines(FineManagementService fineManagementService,
		UserManagementService userManagementService, ICurrentUserSession session,
		IAuthorizationService authorization)
	{
		if (SessionGuard.RequirePermission(authorization, Permission.MyFines, Messages.AccessDenied))
		{
			var fines = fineManagementService.GetFinesByUser(session.UserId!.Value);
			if (fines.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.FineNotFound);
				return;
			}

			FinePrinter.PrintTable(fines);
		}
		else if (SessionGuard.RequirePermission(authorization, Permission.ViewFinesByUser, Messages.AccessDenied) ||
		         SessionGuard.RequirePermission(authorization, Permission.ViewFinesByBook, Messages.AccessDenied))
		{
			Console.WriteLine(new string('=', 36) + " VIEW FINE MENU " + new string('=', 36));

			while (true)
			{
				Console.WriteLine("1. View Fines By User");
				Console.WriteLine("2. View Fines By Book");
				Console.WriteLine("3. Back");

				var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 3);
				if (editMenuChoice == null) return;

				switch (editMenuChoice)
				{
					case 1:
					{
						var user = MenuHelper.SelectUser(userManagementService.GetAllUsers(session));
						if (user is null) break;

						var fines = fineManagementService.GetFinesByUser(user.Id, session);
						DisplayFines(fines, Messages.UserHasNoBorrowedBooks);


						break;
					}
					case 2:
					{
						break;
					}
					case 3:
					{
						break;
					}
				}
			}
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


	private static void DisplayFines(IReadOnlyList<FineDto> fines, string emptyMessage)
	{
		if (fines.Count == 0)
		{
			ConsoleHelper.ShowWarning(emptyMessage);
			return;
		}

		FinePrinter.PrintTable(fines);
	}
}