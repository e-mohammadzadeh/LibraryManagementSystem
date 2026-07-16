using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class UserMenu
{
	public static void UserMenuController(UserManagementService userManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
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
					var desiredUser = SelectExistingUser(userManagementService);
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
			Console.WriteLine("============================ USER MENU ============================");
			Console.WriteLine("1. Register User");
			Console.WriteLine("2. Edit User");
			Console.WriteLine("3. Remove User");
			Console.WriteLine("4. Search User");
			Console.WriteLine("5. View User Details");
			Console.WriteLine("6. View All Users");
			Console.WriteLine("7. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddUser(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING USER MENU ============================");
		var userDto = PromptForUserDto(userManagementService);
		if (userDto is null) return;

		var result = userManagementService.AddUser(userDto);
		ConsoleHelper.ShowResult(result);
	}


	private static CreateUserDto? PromptForUserDto(UserManagementService userManagementService)
	{
		var firstName = ConsoleHelper.GetValidName("Enter user's first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (firstName == null) return null;

		var lastName = ConsoleHelper.GetValidName("Enter user's last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (lastName == null) return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode("Enter user's national code");
		if (nationalCode == null) return null;

		var email = ConsoleHelper.GetValidEmail("Enter user's email");
		if (email == null) return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter user's phone number");
		if (phoneNumber == null) return null;

		var birthDate = ConsoleHelper.GetValidBirthDate("Enter user's birth date");
		if (birthDate == null) return null;

		var availableRoles = userManagementService.GetAllRoles();
		var roleIds = ConsoleHelper.ReadRoles("Select role(s) for this user", availableRoles);
		if (roleIds == null) return null;

		return new CreateUserDto()
		{
			FirstName = firstName,
			LastName = lastName,
			NationalCode = nationalCode,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = birthDate.Value,
			RoleIds = roleIds
		};
	}


	private static void EditUser(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING USER MENU ============================");
		var desiredUser = SelectExistingUser(userManagementService);
		if (desiredUser == null) return;

		while (true)
		{
			Console.WriteLine("{0, -20} [{1}]", "1. First Name", desiredUser.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredUser.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredUser.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredUser.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredUser.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredUser.BirthDate);
			Console.WriteLine("{0, -20} [{1}]", "7. Role", string.Join(", ", desiredUser.UserRoles.Select(ur => ur.Role.Name)));
			Console.WriteLine("8. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt("Enter the number of the field you wish to edit", 1, 8);
			if (editMenuChoice == null)
				return;

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

					var dto = new UpdateUserDto {RoleIds = roleIds};
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

			var choice = ConsoleHelper.ReadYesNo("Do you want to edit another field");
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static User? SelectExistingUser(UserManagementService userManagementService)
	{
		var user = userManagementService.GetAllUsers();
		if (user.Count is not 0) return MenuHelper.SelectUser(user);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableUser);
		return null;
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
		Console.WriteLine("============================ REMOVING USER MENU ============================");
		var desiredUser = SelectExistingUser(userManagementService);
		if (desiredUser == null)
		{
			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
			return;
		}

		UserPrinter.PrintDetails(desiredUser);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredUser.FirstName} {desiredUser.LastName}");

		if (choice != true) return;

		var result = userManagementService.RemoveUser(desiredUser.Id);
		ConsoleHelper.ShowResult(result);
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}


	private static void SearchUser(UserManagementService userManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("============================ SEARCHING USER MENU ============================");
			var usersList = userManagementService.GetAllUsers();
			if (usersList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableUser);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 5);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchUsersAndDisplay(userManagementService, "Enter a name to search",
						user => $"{user.FirstName} {user.LastName}");

					break;
				}
				case 2:
				{
					SearchUsersAndDisplay(userManagementService, "Enter a national code to search",
						user => user.NationalCode);

					break;
				}
				case 3:
				{
					SearchUsersAndDisplay(userManagementService, "Enter an email to search", user => user.Email);
					break;
				}
				case 4:
				{
					SearchUsersAndDisplay(userManagementService, "Enter a phone number to search",
						user => user.PhoneNumber);

					break;
				}
				case 5:
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


	private static void SearchUsersAndDisplay(UserManagementService userManagementService, string prompt,
		Func<User, string?> selector)
	{
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem == null) return;

		var result = userManagementService.SearchUser(searchItem, selector);

		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotUserMatched);
			return;
		}

		UserPrinter.PrintTable(result);
	}
}