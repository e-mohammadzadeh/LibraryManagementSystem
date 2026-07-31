using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class UserMenu
{
	public static void UserMenuController(UserManagementService userManagementService,
		LibraryStatisticsService statisticsService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (UserMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddUser(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					EditUser(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveUser(userManagementService);
					break;
				}
				case 4:
				{
					SearchUser(userManagementService);
					break;
				}
				case 5:
				{
					Console.Clear();
					var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
						MenuHelper.SelectUser, ValidationMessages.NotAvailableUser);
					if (desiredUser is not null)
					{
						UserPrinter.PrintDetails(desiredUser);
						ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
						Console.ReadKey(true);
					}

					break;
				}
				case 6:
				{
					Console.Clear();
					if (userManagementService.GetAllUsers().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableUser);
					else
						UserPrinter.PrintTable(userManagementService.GetAllUsers());

					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to Main Menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int UserMenuList()
	{
		while (true)
		{
			Console.WriteLine(new string('=', 36) + " USER MENU " + new string('=', 36));
			Console.WriteLine("1. Register User");
			Console.WriteLine("2. Edit User");
			Console.WriteLine("3. Remove User");
			Console.WriteLine("4. Search User");
			Console.WriteLine("5. View User Details");
			Console.WriteLine("6. View All Users");
			Console.WriteLine("7. Back");
			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddUser(UserManagementService userManagementService)
	{
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
		if (password is null) return null;

		return new CreateUserDto()
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = fields.Email, PhoneNumber = fields.PhoneNumber, BirthDate = fields.BirthDate, RoleIds = roleIds,
			Password = password
		};
	}


	private static void EditUser(UserManagementService userManagementService)
	{
		Console.WriteLine(new string('=', 36) + " EDITING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
			MenuHelper.SelectUser, ValidationMessages.NotAvailableUser);
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
			var editMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.EditMenuQuestion, 1, 8);
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
					ConsoleHelper.ShowError("Edit cancelled. Returning to User Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(ValidationMessages.EditContinuesQuestion);
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


	private static void RemoveUser(UserManagementService userManagementService)
	{
		Console.WriteLine(new string('=', 36) + " REMOVING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
			MenuHelper.SelectUser, ValidationMessages.NotAvailableUser);

		PersonHelper.PerformRemove(desiredUser, desiredUser?.FirstName ?? "", desiredUser?.LastName ?? "",
			UserPrinter.PrintDetails, () => userManagementService.RemoveUser(desiredUser!.Id));
	}


	private static void SearchUser(UserManagementService userManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING USER MENU " + new string('=', 36));
			var usersList = userManagementService.GetAllUsers();
			if (usersList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableUser);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("{0, -20}", "5. Role");
			Console.WriteLine("6. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.SearchMenuQuestion, 1, 6);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					PersonHelper.SearchAndDisplay("Enter a name to search",
						term => userManagementService.SearchUser(term, user => $"{user.FirstName} {user.LastName}"),
						UserPrinter.PrintTable, ValidationMessages.NotUserMatched);

					break;
				}
				case 2:
				{
					PersonHelper.SearchAndDisplay("Enter a national code to search",
						term => userManagementService.SearchUser(term, user => user.NationalCode),
						UserPrinter.PrintTable, ValidationMessages.NotUserMatched);

					break;
				}
				case 3:
				{
					PersonHelper.SearchAndDisplay("Enter an email to search",
						term => userManagementService.SearchUser(term, user => user.Email),
						UserPrinter.PrintTable, ValidationMessages.NotUserMatched);
					break;
				}
				case 4:
				{
					PersonHelper.SearchAndDisplay("Enter a phone number to search",
						term => userManagementService.SearchUser(term, user => user.PhoneNumber),
						UserPrinter.PrintTable, ValidationMessages.NotUserMatched);

					break;
				}
				case 5:
				{
					SearchRoleAndDisplay(userManagementService, "Enter a role to search");
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo("Search cancelled. Returning to User Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
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
			ConsoleHelper.ShowWarning(ValidationMessages.NotRoleMatched);
			return;
		}

		UserPrinter.PrintTable(result);
	}
}