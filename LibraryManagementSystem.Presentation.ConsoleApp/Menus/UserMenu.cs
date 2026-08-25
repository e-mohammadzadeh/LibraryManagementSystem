using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;
using LibraryManagementSystem.Domain.Entities;

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
			    Permission.ViewOwnDetails,
			    Permission.ViewAllUsers,
			    Permission.ChangePassword,
			    Permission.ChangeOwnPassword,
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
			switch (UserMenuList(authorization, session))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddUser, Messages.AccessDenied))
						break;
					Console.Clear();
					AddUser(userManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditUser, Messages.AccessDenied))
						break;
					Console.Clear();
					EditUser(userManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveUser, Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveUser(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchUser, Messages.AccessDenied))
						break;
					Console.Clear();
					SearchUser(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!session.IsSelfServiceMember)
					{
						ConsoleHelper.ShowError(Messages.AccessDenied);
						break;
					}

					Console.Clear();
					ViewOwnDetails(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.ViewUserDetails, Permission.ViewOwnDetails))
						break;
					Console.Clear();
					ViewUserDetails(userManagementService, session, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllUsers, Messages.AccessDenied))
						break;
					Console.Clear();
					if (userManagementService.GetAllUsers().Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
					else
						UserPrinter.PrintFullTable(userManagementService.GetAllUsers());
					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.RenewLibrarianMembership, Permission.RenewMemberMembership))
						return;
					Console.Clear();
					RenewMembership(userManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 9:
				{
					if (!SessionGuard.RequireAnyPermission(authorization, Messages.AccessDenied,
						    Permission.ChangePassword, Permission.ChangeOwnPassword))
						break;
					Console.Clear();
					ChangePassword(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 10:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int UserMenuList(IAuthorizationService authorization, ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Register User", authorization.HasPermission(Permission.AddUser)),
			(2, "Edit User", authorization.HasPermission(Permission.EditUser)),
			(3, "Remove User", authorization.HasPermission(Permission.RemoveUser)),
			(4, "Search User", authorization.HasPermission(Permission.SearchUser)),
			(5, "View Own Details", session.IsSelfServiceMember),
			(6, "View User Details",
				authorization.HasAnyPermission(Permission.ViewUserDetails, Permission.ViewOwnDetails)),
			(7, "View All Users", authorization.HasPermission(Permission.ViewAllUsers)),
			(8, "Renew Membership",
				authorization.HasAnyPermission(Permission.RenewMemberMembership, Permission.RenewLibrarianMembership)),
			(9, "Change Password",
				authorization.HasAnyPermission(Permission.ChangePassword, Permission.ChangeOwnPassword)),
			(10, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " USER MENU " + new string('=', 36));

			var displayNumber = 1;
			foreach (var (_, displayText, _) in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {displayText}");
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


	private static void AddUser(UserManagementService userManagementService, IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.AddUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " ADDING USER MENU " + new string('=', 36));
		var userDto = PromptForUserDto(userManagementService, authorization);
		if (userDto is null) return;

		var result = userManagementService.AddUser(userDto);
		ConsoleHelper.ShowResult(result);
	}


	private static CreateUserDto? PromptForUserDto(UserManagementService userManagementService,
		IAuthorizationService authorization)
	{
		var fields = PersonHelper.PromptForPersonFields("user");
		if (fields is null) return null;

		var availableRoles = GetAssignableRoles(userManagementService, authorization);
		if (availableRoles.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NoRolesToAssign);
			return null;
		}

		var allowMultiple = authorization.HasAnyPermission(Permission.AssignLibrarianRole, Permission.AssignAdminRole);

		var roleIds =
			ConsoleHelper.ReadRoles("Select role(s) for this user", availableRoles, allowMultiple: allowMultiple);
		if (roleIds == null) return null;

		var password = ConsoleHelper.GetValidPassword(Messages.PasswordPrompt);

		return new CreateUserDto()
		{
			FirstName = fields.FirstName, LastName = fields.LastName, NationalCode = fields.NationalCode,
			Email = fields.Email, PhoneNumber = fields.PhoneNumber, BirthDate = fields.BirthDate, RoleIds = roleIds,
			Password = password
		};
	}


	private static IReadOnlyList<Role> GetAssignableRoles(UserManagementService userManagementService,
		IAuthorizationService authorization)
	{
		var all = userManagementService.GetAllRoles();
		return
		[
			.. all
				.Where(role => role.Name switch
				{
					LibraryUserRole.Member => authorization.HasPermission(Permission.AssignMemberRole),
					LibraryUserRole.Librarian => authorization.HasPermission(Permission.AssignLibrarianRole),
					LibraryUserRole.Admin => authorization.HasPermission(Permission.AssignAdminRole),
					_ => false
				})
		];
	}



	private static void EditUser(UserManagementService userManagementService, IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.EditUser))
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
			Console.Clear();
			var headers = new[] { "#", "Field", "Current Value" };

			var rows = new List<string[][]>
			{
				new string[][] { ["1"], ["First Name"], [desiredUser.FirstName] },
				new string[][] { ["2"], ["Last Name"], [desiredUser.LastName] },
				new string[][] { ["3"], ["National Code"], [desiredUser.NationalCode] },
				new string[][] { ["4"], ["Email"], [desiredUser.Email] },
				new string[][] { ["5"], ["Phone Number"], [desiredUser.PhoneNumber] },
				new string[][] { ["6"], ["Birth Date"], [desiredUser.BirthDate.ToString("yyyy-MM-dd")] },
			};

			if (authorization.HasPermission(Permission.ChangeUserRoles))
				rows.Add(new string[][] { ["7"], ["Roles"], [string.Join(", ", desiredUser.Roles)] });
			rows.Add(new string[][] { [(rows.Count + 1).ToString()], ["Back"], ["-"] });

			ConsoleTable.PrintTable("Edit User", headers, rows);

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, rows.Count);
			if (editMenuChoice == null) return;

			var selectedRow = rows[editMenuChoice.Value - 1];
			var actionId = int.Parse(selectedRow[0][0]);

			switch (actionId)
			{
				case 1:
				{
					var userNewFirstName = ConsoleHelper.GetValidName("\nEnter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewFirstName,
						v => new UpdateUserDto { FirstName = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 2:
				{
					var userNewLastName = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewLastName,
						v => new UpdateUserDto { LastName = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 3:
				{
					var userNewNationalCode = ConsoleHelper.GetValidNationalCode("\nEnter new national code");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewNationalCode,
						v => new UpdateUserDto { NationalCode = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 4:
				{
					var userNewEmail = ConsoleHelper.GetValidEmail("\nEnter new email");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewEmail,
						v => new UpdateUserDto { Email = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 5:
				{
					var userNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewPhoneNumber,
						v => new UpdateUserDto { PhoneNumber = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 6:
				{
					var userNewBirthDate = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");
					var updated = PerformUpdate(userManagementService, desiredUser.Id, userNewBirthDate,
						v => new UpdateUserDto { BirthDate = v });
					if (updated is not null) desiredUser = updated;
					break;
				}
				case 7:
				{
					if (!authorization.HasPermission(Permission.ChangeUserRoles) &&
					    !authorization.HasAnyPermission(Permission.AssignLibrarianRole, Permission.AssignAdminRole))
					{
						ConsoleHelper.ShowError(Messages.AccessDenied);
						break;
					}

					var availableRoles = userManagementService.GetAllRoles();
					var roleIds = ConsoleHelper.ReadRoles("\nSelect role(s) for this user", availableRoles);
					if (roleIds is null) break;

					var result =
						userManagementService.UpdateUser(desiredUser.Id, new UpdateUserDto { RoleIds = roleIds });
					ConsoleHelper.ShowResult(result);
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "User"));
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
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

		PersonHelper.PerformRemove(desiredUser, desiredUser.FirstName, desiredUser.LastName,
			user => UserPrinter.PrintDetails(user),
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
				return;
			}

			Console.WriteLine("{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("{0, -20}", "5. Role");
			Console.WriteLine("6. Back");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 6);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					PersonHelper.SearchAndDisplay(Messages.SearchName,
						term => userManagementService.SearchUser(term, user => $"{user.FirstName} {user.LastName}"),
						UserPrinter.PrintFullTable, Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					PersonHelper.SearchAndDisplay(Messages.SearchNationalCode,
						term => userManagementService.SearchUser(term, user => user.NationalCode),
						UserPrinter.PrintFullTable, Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					PersonHelper.SearchAndDisplay(Messages.SearchEmail,
						term => userManagementService.SearchUser(term, user => user.Email),
						UserPrinter.PrintFullTable, Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					PersonHelper.SearchAndDisplay(Messages.SearchPhoneNumber,
						term => userManagementService.SearchUser(term, user => user.PhoneNumber),
						UserPrinter.PrintFullTable, Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					SearchRoleAndDisplay(userManagementService, "Enter a role to search");
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "User"));
					return;
				}
			}
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

		UserPrinter.PrintFullTable(result);
	}


	private static void ViewUserDetails(UserManagementService userManagementService, ICurrentUserSession session,
		IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.ViewOwnDetails) &&
		    !authorization.HasPermission(Permission.ViewUserDetails))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " USER DETAILS " + new string('=', 36));
			Console.WriteLine("1. View Own Details");
			Console.WriteLine("2. View User Details");
			Console.WriteLine("3. Back");
			Console.WriteLine(new string('=', 82));

			var choice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 3);
			if (choice is null) return;
			switch (choice)
			{
				case 1:
				{
					Console.Clear();
					ViewOwnDetails(userManagementService, session);
					ConsoleHelper.Pause();
					break;
				}

				case 2:
				{
					Console.Clear();
					var userDto = MenuHelper.SelectExisting(userManagementService.GetAllUsers(), MenuHelper.SelectUser,
						Messages.NotAvailableUser);
					if (userDto is null) return;
					UserPrinter.PrintDetails(userDto);
					ConsoleHelper.Pause();
					break;
				}

				case 3:
				{
					return;
				}
			}
		}
	}


	private static void ViewOwnDetails(UserManagementService userManagementService, ICurrentUserSession session)
	{
		if (session.UserId is null)
		{
			ConsoleHelper.ShowError(Messages.AuthenticationRequired);
			return;
		}

		var userDto = userManagementService.FindUserById(session.UserId.Value);
		if (userDto is null)
		{
			ConsoleHelper.ShowError(Messages.NotUserMatched);
			return;
		}

		UserPrinter.PrintDetails(userDto, "My Details");
	}


	private static void RenewMembership(UserManagementService userManagementService,
		IAuthorizationService authorization)
	{
		Console.WriteLine(new string('=', 56) + " RENEW MEMBERSHIP " + new string('=', 56));

		var canRenewMembers = authorization.HasPermission(Permission.RenewMemberMembership);
		var canRenewLibrarians = authorization.HasPermission(Permission.RenewLibrarianMembership);

		var allUsers = userManagementService.GetAllUsers();

		var renewableUsers = allUsers
			.Where(u =>
			{
				var allRolesRenewable = u.Roles.All(role =>
					(role == LibraryUserRole.Member && canRenewMembers) ||
					(role == LibraryUserRole.Librarian && canRenewLibrarians));

				var hasRenewableRole = u.Roles.Any(role => role is LibraryUserRole.Member or LibraryUserRole.Librarian);

				return allRolesRenewable && hasRenewableRole;
			}).ToList();

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
		if (currentPassword is null) return;
		var newPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "new"));
		if (newPassword is null) return;
		var confirmPassword = ConsoleHelper.GetValidPassword(Messages.PasswordConfirmation);
		if (confirmPassword is null) return;

		if (newPassword != confirmPassword)
		{
			ConsoleHelper.ShowError(Messages.PasswordMatchedFailed);
			return;
		}

		var result = userManagementService.ChangePassword(session.UserId.Value, currentPassword, newPassword, session);
		ConsoleHelper.ShowResult(result);
	}
}