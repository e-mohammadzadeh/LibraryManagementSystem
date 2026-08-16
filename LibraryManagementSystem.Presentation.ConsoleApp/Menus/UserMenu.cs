using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class UserMenu
{
	public static void UserMenuController(UserManagementService userManagementService,
		LibraryStatisticsService statisticsService, ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.AddUser,
			    Permission.EditUser,
			    Permission.RemoveUser,
			    Permission.SearchUser,
			    Permission.ViewUserDetails,
			    Permission.ViewAllUsers,
			    Permission.ChangePassword,
			    Permission.RenewLibrarianMembership,
			    Permission.RenewMemberMembership))
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
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (UserMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddUser, Messages.AccessDenied))
						break;
					AddUser(userManagementService, session);
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditUser, Messages.AccessDenied))
						break;
					EditUser(userManagementService, session);
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveUser, Messages.AccessDenied))
						break;
					RemoveUser(userManagementService, session);
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchUser, Messages.AccessDenied))
						break;
					SearchUser(userManagementService, session);
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewUserDetails,
						    Messages.AccessDenied))
						break;
					ViewUserDetails(userManagementService, session);
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllUsers, Messages.AccessDenied))
						break;
					if (userManagementService.GetAllUsers().Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
					else
						UserPrinter.PrintTable(userManagementService.GetAllUsers());
					break;
				}
				case 7:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.RenewLibrarianMembership, Permission.RenewMemberMembership))
						return;
					RenewMembership(userManagementService, authorization);
					break;
				}
				case 8:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ChangePassword,
						    Messages.AccessDenied))
						break;
					ChangePassword(userManagementService, session);
					break;
				}
				case 9:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static int UserMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Register User", authorization.HasPermission(Permission.AddUser)),
			(2, "Edit User", authorization.HasPermission(Permission.EditUser)),
			(3, "Remove User", authorization.HasPermission(Permission.RemoveUser)),
			(4, "Search User", authorization.HasPermission(Permission.SearchUser)),
			(5, "View User Details", authorization.HasPermission(Permission.ViewUserDetails)),
			(6, "View All Users", authorization.HasPermission(Permission.ViewAllUsers)),
			(7, "Renew Membership",
				authorization.HasAnyPermission(Permission.RenewMemberMembership, Permission.RenewLibrarianMembership)),
			(8, "Change Password", authorization.HasPermission(Permission.ChangePassword)),
			(9, "Back", true)
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
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
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

					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewFirstName,
						v => new UpdateUserDto { FirstName = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 2:
				{
					var userNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewLastName,
						v => new UpdateUserDto { LastName = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 3:
				{
					var userNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewNationalCode,
						v => new UpdateUserDto { NationalCode = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 4:
				{
					var userNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewEmail,
						v => new UpdateUserDto { Email = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 5:
				{
					var userNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewPhoneNumber,
						v => new UpdateUserDto { PhoneNumber = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 6:
				{
					var userNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewBirthDate,
						v => new UpdateUserDto { BirthDate = v });
					if (updated is not null) desiredUser = updated;
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
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "User"));
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


	private static UserDto? PerformUpdate<T>(UserManagementService userManagementService, int desiredMemberId,
		T? newValue,
		Func<T, UpdateUserDto> buildDto)
	{
		if (newValue is null) return null;
		var dto = buildDto(newValue);
		var result = userManagementService.UpdateUser(desiredMemberId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Data;
	}


	private static void RemoveUser(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " REMOVING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
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
			var usersList = userManagementService.GetAllUsers();
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
			userDto = MenuHelper.SelectExisting(userManagementService.GetAllUsers(), MenuHelper.SelectUser,
				Messages.NotAvailableUser);
		}

		if (userDto is null)
		{
			ConsoleHelper.ShowError(Messages.NotUserMatched);
			return;
		}

		UserPrinter.PrintDetails(userDto);
	}


	private static void RenewMembership(UserManagementService userManagementService, IAuthorizationService authorization)
	{
		Console.WriteLine(new string('=', 36) + " RENEW MEMBERSHIP " + new string('=', 36));

		var canRenewMembers = authorization.HasPermission(Permission.RenewMemberMembership);
		var canRenewLibrarians = authorization.HasPermission(Permission.RenewLibrarianMembership);

		var allUsers = userManagementService.GetAllUsers();

		var renewableUsers = allUsers
			.Where(u => (canRenewMembers && u.Roles.Contains(LibraryUserRole.Member)) ||
			            (canRenewLibrarians && u.Roles.Contains(LibraryUserRole.Librarian))).ToList();

		if (renewableUsers.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NoRenewableUser);
			return;
		}

		var selectedUser = MenuHelper.SelectExisting(renewableUsers, MenuHelper.SelectUser, Messages.NotAvailableUser);
		if (selectedUser is null) return;

		var years = ConsoleHelper.ReadInt(Messages.EnterYearForRenewMembership,
			ValidationConstants.MinRenewMembershipYear, ValidationConstants.MaxRenewMembershipYear);
		if (years is null) return;

		var result = userManagementService.RenewMembership(selectedUser.Id, years.Value);
		ConsoleHelper.ShowResult(result);
	}


	private static void ChangePassword(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (!session.IsAuthenticated || session.UserId == null)
		{
			ConsoleHelper.ShowError(Messages.SessionExpired);
			return;
		}

		Console.WriteLine(new string('=', 36) + " CHANGE PASSWORD " + new string('=', 36));

		var currentPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "current"));
		var newPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "new"));
		var confirmPassword = ConsoleHelper.GetValidPassword(Messages.PasswordConfirmation);

		if (newPassword != confirmPassword)
		{
			ConsoleHelper.ShowError(Messages.PasswordMatchedFailed);
			return;
		}

		var result = userManagementService.ChangePassword(session.UserId.Value, currentPassword, newPassword, session);
		ConsoleHelper.ShowResult(result);
	}
}