using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class UserMenu
{
	public static void UserMenuController(UserManagementService userManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session)
	{
		if (!SessionGuard.RequireUserManagement(session)) return;

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
			switch (UserMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					AddUser(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					EditUser(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveUser(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					SearchUser(userManagementService, session);
					break;
				}
				case 5:
				{
					Console.Clear();
					ViewUserDetails(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					if (session is { IsAdmin: false, IsLibrarian: false })
					{
						ConsoleHelper.ShowError(Messages.AccessDenied);
						return;
					}

					if (userManagementService.GetAllUsers(session).Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
					else
						UserPrinter.PrintTable(userManagementService.GetAllUsers(session));

					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					
					break;
				}
				case 8:
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


	private static int UserMenuList(ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Register User", session.IsAdmin || session.IsLibrarian),
			(2, "Edit User", session.IsAdmin || session.IsLibrarian),
			(3, "Remove User", session.IsAdmin || session.IsLibrarian),
			(4, "Search User", session.IsAdmin || session.IsLibrarian),
			(5, "View User Details", true),
			(6, "View All Users", session.IsAdmin || session.IsLibrarian),
			(7, "Change Password", true),
			(8, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " USER MENU " + new string('=', 36));

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


	private static void AddUser(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " ADDING USER MENU " + new string('=', 36));
		var userDto = PromptForUserDto(userManagementService);
		if (userDto is null) return;

		var result = userManagementService.AddUser(userDto);
		ConsoleHelper.ShowResult(result);
	}


	private static CreateUserDto? PromptForUserDto(UserManagementService userManagementService)
	{
		var fields = PersonHelper.PromptForPersonFields("user");
		if (fields is null) return null;

		var availableRoles = userManagementService.GetAllRoles();
		var roleIds = ConsoleHelper.ReadRoles("Select role(s) for this user", availableRoles);
		if (roleIds == null) return null;

		var password = ConsoleHelper.GetValidPassword("Enter a password for the user's login account");

		return new CreateUserDto()
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = fields.Email, PhoneNumber = fields.PhoneNumber, BirthDate = fields.BirthDate, RoleIds = roleIds,
			Password = password
		};
	}


	private static void EditUser(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " EDITING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(session),
			MenuHelper.SelectUser, Messages.NotAvailableUser);
		if (desiredUser == null) return;

		while (true)
		{
			Console.WriteLine("\n{0, -20} [{1}]", "1. First Name", desiredUser.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredUser.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredUser.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredUser.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredUser.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredUser.BirthDate);
			Console.WriteLine("{0, -20} [{1}]", "7. Role", string.Join(", ", desiredUser.Roles));
			Console.WriteLine("8. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 8);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var userNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredUser.Id, userNewFirstName,
						v => new UpdateUserDto { FirstName = v });

					break;
				}
				case 2:
				{
					var userNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredUser.Id, userNewLastName,
						v => new UpdateUserDto { LastName = v });

					break;
				}
				case 3:
				{
					var userNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					PerformUpdate(userManagementService, desiredUser.Id, userNewNationalCode,
						v => new UpdateUserDto { NationalCode = v });

					break;
				}
				case 4:
				{
					var userNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					PerformUpdate(userManagementService, desiredUser.Id, userNewEmail,
						v => new UpdateUserDto { Email = v });

					break;
				}
				case 5:
				{
					var userNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					PerformUpdate(userManagementService, desiredUser.Id, userNewPhoneNumber,
						v => new UpdateUserDto { PhoneNumber = v });

					break;
				}
				case 6:
				{
					var userNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					PerformUpdate(userManagementService, desiredUser.Id, userNewBirthDate,
						v => new UpdateUserDto { BirthDate = v });

					break;
				}
				case 7:
				{
					var availableRoles = userManagementService.GetAllRoles();
					var roleIds = ConsoleHelper.ReadRoles("Select role(s) for this user", availableRoles);
					if (roleIds is null) break;

					var dto = new UpdateUserDto { RoleIds = roleIds };
					var result = userManagementService.UpdateUser(desiredUser.Id, dto);
					ConsoleHelper.ShowResult(result);
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowError(string.Format(Messages.EditCancelled, "User"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static void PerformUpdate<T>(UserManagementService userManagementService, int desiredMemberId, T? newValue,
		Func<T, UpdateUserDto> buildDto)
	{
		if (newValue is null) return;
		var dto = buildDto(newValue);
		var result = userManagementService.UpdateUser(desiredMemberId, dto);
		ConsoleHelper.ShowResult(result);
	}


	private static void RemoveUser(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " REMOVING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(session),
			MenuHelper.SelectUser, Messages.NotAvailableUser);
		if (desiredUser is null) return;

		PersonHelper.PerformRemove(desiredUser, desiredUser.FirstName, desiredUser.LastName, UserPrinter.PrintDetails,
			() => userManagementService.RemoveUser(desiredUser.Id, session));
	}


	private static void SearchUser(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING USER MENU " + new string('=', 36));
			var usersList = userManagementService.GetAllUsers(session);
			if (usersList.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
				ConsoleHelper.Pause();
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("{0, -20}", "5. Role");
			Console.WriteLine("6. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 6);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					PersonHelper.SearchAndDisplay("Enter a name to search",
						term => userManagementService.SearchUser(term, user => $"{user.FirstName} {user.LastName}"),
						UserPrinter.PrintTable, Messages.NotUserMatched);

					break;
				}
				case 2:
				{
					PersonHelper.SearchAndDisplay("Enter a national code to search",
						term => userManagementService.SearchUser(term, user => user.NationalCode),
						UserPrinter.PrintTable, Messages.NotUserMatched);

					break;
				}
				case 3:
				{
					PersonHelper.SearchAndDisplay("Enter an email to search",
						term => userManagementService.SearchUser(term, user => user.Email),
						UserPrinter.PrintTable, Messages.NotUserMatched);
					break;
				}
				case 4:
				{
					PersonHelper.SearchAndDisplay("Enter a phone number to search",
						term => userManagementService.SearchUser(term, user => user.PhoneNumber),
						UserPrinter.PrintTable, Messages.NotUserMatched);

					break;
				}
				case 5:
				{
					SearchRoleAndDisplay(userManagementService, "Enter a role to search");
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "User"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static void SearchRoleAndDisplay(UserManagementService userManagementService, string prompt)
	{
		var availableRoles = userManagementService.GetAllRoles();
		var roleId = ConsoleHelper.ReadRoles(prompt, availableRoles, false);
		if (roleId is null) return;

		var result = userManagementService.SearchByRole(roleId);
		DisplayUserResults(result);
	}


	private static void DisplayUserResults(IReadOnlyList<UserDto> result)
	{
		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotRoleMatched);
			return;
		}

		UserPrinter.PrintTable(result);
	}


	private static void ViewUserDetails(UserManagementService userManagementService, ICurrentUserSession session)
	{
		UserDto? userDto;
		if (session.IsAuthenticated)
		{
			userDto = userManagementService.FindUserById(session.UserId!.Value);
		}
		else
		{
			userDto = MenuHelper.SelectExisting(userManagementService.GetAllUsers(session), MenuHelper.SelectUser,
				Messages.NotAvailableUser);
		}

		if (userDto is null)
		{
			ConsoleHelper.ShowError(Messages.NotUserMatched);
			return;
		}

		UserPrinter.PrintDetails(userDto);
	}
}