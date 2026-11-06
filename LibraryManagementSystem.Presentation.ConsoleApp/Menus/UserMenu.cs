using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Role;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class UserMenu
{
	public static void UserMenuController(UserManagementService userManagementService,
		RoleManagementService roleManagementService, LibraryStatisticsService statisticsService,
		ICurrentUserSession session, IAuthorizationService authorization, IMenuService menuService)
	{
		if (!authorization.HasAnyPermission(Permission.AddUser,
			    Permission.EditUser, Permission.RemoveUser, Permission.SearchUser, Permission.SortUser,
			    Permission.ViewUserDetails, Permission.ViewAllUsers, Permission.ChangePassword,
			    Permission.RenewLibrarianMembership, Permission.RenewMemberMembership))
			return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (UserMenuList(menuService))
			{
				case "users.register":
				{
					Console.Clear();
					AddUser(userManagementService, roleManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "users.edit":
				{
					Console.Clear();
					EditUser(userManagementService, roleManagementService, authorization, session);
					ConsoleHelper.Pause();
					break;
				}
				case "users.remove":
				{
					Console.Clear();
					RemoveUser(userManagementService, authorization, session);
					ConsoleHelper.Pause();
					break;
				}
				case "users.search":
				{
					Console.Clear();
					SearchUser(userManagementService, menuService, roleManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "users.sort":
				{
					Console.Clear();
					SortUsers(userManagementService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "users.view":
				{
					if (!authorization.HasAnyPermission(Permission.ViewUserDetails, Permission.ViewAllUsers,
						    Permission.ViewRemovedUsers))
						return;
					Console.Clear();
					ViewUsers(userManagementService, authorization, session, statisticsService, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "users.renew":
				{
					if (!authorization.HasAnyPermission(Permission.RenewLibrarianMembership,
						    Permission.RenewMemberMembership))
						return;
					Console.Clear();
					RenewMembership(userManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "users.change-password":
				{
					Console.Clear();
					ChangePassword(userManagementService, session, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "users.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? UserMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("users");
		return MenuHelper.DisplayMenu("USER MENU", items);
	}


	private static void AddUser(UserManagementService userManagementService,
		RoleManagementService roleManagementService, IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.AddUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " ADDING USER MENU " + new string('=', 36));
		var userDto = PromptForUserDto(userManagementService, roleManagementService);
		if (userDto is null) return;

		var result = userManagementService.AddUser(userDto);
		ConsoleHelper.ShowResult(result);
	}


	private static UserDto? PromptForUserDto(UserManagementService userManagementService,
		RoleManagementService roleManagementService)
	{
		var fields = PersonHelper.PromptForPersonFields("user");
		if (fields is null) return null;

		var availableRoles = GetAssignableRoles(userManagementService, roleManagementService);
		if (availableRoles.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NoRolesToAssign);
			return null;
		}

		//var allowMultiple = authorization.HasAnyPermission(Permission.AssignLibrarianRole, Permission.AssignAdminRole);

		var roles = ConsoleHelper.ReadRoles("Select role for this user", availableRoles, allowMultiple: false);
		if (roles is null || roles.Count == 0) return null;

		var newPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "new"));
		if (newPassword is null) return null;

		var confirmPassword = ConsoleHelper.GetValidPassword(Messages.PasswordConfirmation);
		if (confirmPassword is null) return null;

		if (newPassword != confirmPassword)
		{
			ConsoleHelper.ShowError(Messages.PasswordMatchedFailed);
			return null;
		}

		return new UserDto
		{
			FirstName = fields.FirstName,
			LastName = fields.LastName,
			NationalCode = fields.NationalCode,
			Email = Email.Create(fields.Email),
			PhoneNumber = PhoneNumber.Create(fields.PhoneNumber),
			BirthDate = fields.BirthDate,
			RoleName = roles[0].Name,
			Password = newPassword
		};
	}


	private static IReadOnlyList<RoleDto> GetAssignableRoles(UserManagementService userManagementService
		, RoleManagementService roleManagementService)
	{
		return
		[
			.. roleManagementService.GetAllRoles()
				.Where(role => userManagementService.CanAssignRole(role.Name))
				.Select(role => role.ToDto())
		];
	}


	private static void EditUser(UserManagementService userManagementService,
		RoleManagementService roleManagementService, IAuthorizationService authorization, ICurrentUserSession session)
	{
		var canEditAnyUser = authorization.HasPermission(Permission.EditUser);
		var canEditOwnProfile = authorization.HasPermission(Permission.EditOwnProfile);

		if (!canEditAnyUser && !canEditOwnProfile)
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		UserDto? desiredUser;
		var isSelfServiceEdit = false;

		if (!canEditAnyUser)
		{
			desiredUser = userManagementService.GetUserById(session.CurrentUser.Id);
			if (desiredUser is null)
			{
				ConsoleHelper.ShowError(Messages.NotAvailableUser);
				return;
			}

			isSelfServiceEdit = true;
		}
		else
		{
			Console.WriteLine(new string('=', 36) + " EDITING USER MENU " + new string('=', 36));
			desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
				list => MenuHelper.SelectUser(list, users => UserPrinter.PrintTable(users)),
				Messages.NotAvailableUser);
			if (desiredUser is null) return;
		}

		EditUserLoop(userManagementService, roleManagementService, authorization, session, desiredUser,
			isSelfServiceEdit);
	}


	private static void EditUserLoop(UserManagementService userManagementService,
		RoleManagementService roleManagementService, IAuthorizationService authorization,
		ICurrentUserSession session, UserDto desiredUser, bool isSelfServiceEdit)
	{
		while (true)
		{
			Console.Clear();
			var rows = new List<string[][]>
			{
				new string[][] { ["1"], ["First Name"], [desiredUser.FirstName] },
				new string[][] { ["2"], ["Last Name"], [desiredUser.LastName] },
				new string[][] { ["3"], ["National Code"], [desiredUser.NationalCode] },
			};

			if (!isSelfServiceEdit) rows.Add([["4"], ["Email"], [desiredUser.Email]]);

			rows.Add([[(rows.Count + 1).ToString()], ["Phone Number"], [desiredUser.PhoneNumber]]);
			rows.Add([[(rows.Count + 1).ToString()], ["Birth Date"], [desiredUser.BirthDate.ToString("yyyy-MM-dd")]]);

			var canChangeRole = !isSelfServiceEdit && authorization.HasPermission(Permission.EditUserRoles);
			if (canChangeRole) rows.Add([[(rows.Count + 1).ToString()], ["Role"], [desiredUser.RoleName.ToString()]]);

			rows.Add([[(rows.Count + 1).ToString()], ["Back"], ["-"]]);

			ConsoleTable.PrintTable("Edit User", ["#", "Field", "Current Value"], rows);

			var choice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, rows.Count);
			if (choice is null) return;

			var fieldName = rows[choice.Value - 1][1][0];

			switch (fieldName)
			{
				case "First Name":
				{
					var userNewFirstName = ConsoleHelper.GetValidName("\nEnter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);
					if (userNewFirstName != null)
					{
						desiredUser.FirstName = userNewFirstName;
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Last Name":
				{
					var userNewLastName = ConsoleHelper.GetValidName("\nEnter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);
					if (userNewLastName != null)
					{
						desiredUser.LastName = userNewLastName;
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "National Code":
				{
					var userNewNationalCode = ConsoleHelper.GetValidNationalCode("\nEnter new national code");
					if (userNewNationalCode != null)
					{
						desiredUser.NationalCode = userNewNationalCode;
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Email":
				{
					var userNewEmail = ConsoleHelper.GetValidEmail("\nEnter new email");
					if (userNewEmail != null)
					{
						desiredUser.Email = Email.Create(userNewEmail);
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Phone Number":
				{
					var userNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");
					if (userNewPhoneNumber != null)
					{
						desiredUser.PhoneNumber = PhoneNumber.Create(userNewPhoneNumber);
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Birth Date":
				{
					var newBirthDate = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");
					if (newBirthDate is not null)
					{
						desiredUser.BirthDate = newBirthDate.Value;
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Role":
				{
					var availableRoles = roleManagementService.GetAllRoles().Select(r => r.ToDto()).ToList();
					var newRole = ConsoleHelper.ReadRoles("\nSelect role for this user", availableRoles,
						allowMultiple: false);
					if (newRole != null)
					{
						desiredUser.RoleName = newRole[0].Name;
						var result = userManagementService.UpdateUser(desiredUser, session, session.CurrentUser.Id);
						ConsoleHelper.ShowResult(result);
					}

					break;
				}
				case "Back":
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "User"));
					return;
			}

			var continueEditing = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (continueEditing != true) return;
		}
	}



	private static void RemoveUser(UserManagementService userManagementService, IAuthorizationService authorization,
		ICurrentUserSession session)
	{
		if (!authorization.HasPermission(Permission.RemoveUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " REMOVING USER MENU " + new string('=', 36));
		var desiredUser = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
			list => MenuHelper.SelectUser(list, users => UserPrinter.PrintTable(users)), Messages.NotAvailableUser);
		if (desiredUser is null) return;

		// Block removing self
		if (session.CurrentUser.Id == desiredUser.Id)
		{
			ConsoleHelper.ShowError(Messages.CannotRemoveYourself);
			return;
		}

		PersonHelper.PerformRemove(desiredUser, desiredUser.FirstName, desiredUser.LastName,
			user => UserPrinter.PrintDetails(user),
			() => userManagementService.RemoveUser(desiredUser.Id, session));
	}


	private static void SearchUser(UserManagementService userManagementService, IMenuService menuService,
		RoleManagementService roleManagementService, IAuthorizationService authorization)
	{
		if (!authorization.HasPermission(Permission.SearchUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		if (userManagementService.GetAllUsers().Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
			return;
		}

		while (true)
		{
			Console.Clear();
			var selectedId = MenuHelper.DisplayMenu("SEARCHING USER MENU", menuService.GetChildren("users.search"));

			if (selectedId is null) return;

			if (selectedId.Equals("users.search.back", StringComparison.OrdinalIgnoreCase))
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "User"));
				return;
			}

			switch (selectedId.ToLowerInvariant())
			{
				case "users.search.name":
				{
					PersonHelper.SearchAndDisplay(Messages.SearchName,
						term => userManagementService.SearchUser(term, UserSearchField.FullName),
						u => UserPrinter.PrintFullTable(u), Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case "users.search.national-code":
				{
					PersonHelper.SearchAndDisplay(Messages.SearchNationalCode,
						term => userManagementService.SearchUser(term, UserSearchField.NationalCode),
						u => UserPrinter.PrintFullTable(u), Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case "users.search.email":
				{
					PersonHelper.SearchAndDisplay(Messages.SearchEmail,
						term => userManagementService.SearchUser(term, UserSearchField.Email),
						u => UserPrinter.PrintFullTable(u), Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case "users.search.phone-number":
				{
					PersonHelper.SearchAndDisplay(Messages.SearchPhoneNumber,
						term => userManagementService.SearchUser(term, UserSearchField.PhoneNumber),
						u => UserPrinter.PrintFullTable(u), Messages.NotUserMatched);
					ConsoleHelper.Pause();
					break;
				}
				case "users.search.role":
				{
					SearchRoleAndDisplay(userManagementService, roleManagementService, "Enter a role to search");
					ConsoleHelper.Pause();
					break;
				}
				default:
				{
					ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
					ConsoleHelper.Pause();
					break;
				}
			}
		}
	}


	private static void SearchRoleAndDisplay(UserManagementService userManagementService,
		RoleManagementService roleManagementService, string prompt)
	{
		var availableRoles = roleManagementService.GetAllRoles().Select(r => r.ToDto()).ToList();
		var roles = ConsoleHelper.ReadRoles(prompt, availableRoles, allowMultiple: false);
		if (roles is null || roles.Count == 0) return;

		var result = userManagementService.SearchByRole(roles[0].Id);
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


	private static void SortUsers(UserManagementService userManagementService, IAuthorizationService authorization,
		IMenuService menuService)
	{
		if (!authorization.HasPermission(Permission.SortUser))
		{
			ConsoleHelper.ShowError(Messages.AccessDenied);
			return;
		}

		MenuHelper.SortByMenu<UserSortField, UserDto>(
			menuService,
			sortMenuId: "users.sort",
			menuTitle: "SORT USERS",
			mapField: TryMapUserSortField,
			loadSorted: userManagementService.GetAllUsers,
			display: (users, description) =>
				UserPrinter.PrintFullTable(users, $"Sorted Users - {description}"),
			emptyMessage: Messages.NotAvailableUser,
			entityNameForCancel: "User",
			backMenuId: "users.sort.back");
	}


	private static bool TryMapUserSortField(string menuId, out UserSortField field)
	{
		switch (menuId.ToLowerInvariant())
		{
			case "users.sort.firstname":
				field = UserSortField.FirstName;
				return true;
			case "users.sort.lastname":
				field = UserSortField.LastName;
				return true;
			case "users.sort.fullname":
				field = UserSortField.FullName;
				return true;
			case "users.sort.national-code":
				field = UserSortField.NationalCode;
				return true;
			case "users.sort.email":
				field = UserSortField.Email;
				return true;
			case "users.sort.birthdate":
				field = UserSortField.BirthDate;
				return true;
			case "users.sort.role":
				field = UserSortField.Role;
				return true;
			case "users.sort.membership-start":
				field = UserSortField.MembershipStartDate;
				return true;
			case "users.sort.membership-expiry":
				field = UserSortField.MembershipExpiryDate;
				return true;
			case "users.sort.last-login":
				field = UserSortField.LastLoginDate;
				return true;
			default:
				field = default;
				return false;
		}
	}


	private static void ViewUsers(UserManagementService userManagementService, IAuthorizationService authorization,
		ICurrentUserSession session, LibraryStatisticsService statisticsService, IMenuService menuService)
	{
		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (ViewUserMenuList(menuService))
			{
				case "users.view.own-details":
				{
					Console.Clear();
					var userDto = userManagementService.GetUserById(session.CurrentUser.Id);
					if (userDto is null)
					{
						ConsoleHelper.ShowError(Messages.NotUserMatched);
						break;
					}

					UserPrinter.PrintDetails(userDto, "My Details");
					if (userDto.ShouldRemove) ConsoleHelper.ShowError(Messages.AccountScheduledForRemoval);
					ConsoleHelper.Pause();
					break;
				}
				case "users.view.user-details":
				{
					if (!authorization.HasPermission(Permission.ViewUserDetails)) break;
					Console.Clear();
					var userDto = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
						list => MenuHelper.SelectUser(list, users => UserPrinter.PrintTable(users)),
						Messages.NotAvailableUser);
					if (userDto is null) break;
					UserPrinter.PrintDetails(userDto);
					ConsoleHelper.Pause();
					break;
				}
				case "users.view.all-users":
				{
					if (!authorization.HasPermission(Permission.ViewAllUsers)) break;
					Console.Clear();
					var users = userManagementService.GetAllUsers();
					if (users.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableUser);
					else
						UserPrinter.PrintFullTable(users);
					ConsoleHelper.Pause();
					break;
				}
				case "users.view.removed-users":
				{
					if (!authorization.HasPermission(Permission.ViewRemovedUsers)) break;
					Console.Clear();
					var removedUsers = userManagementService.GetRemovedUsers();
					if (removedUsers.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableRemovedUser);
					else
						UserPrinter.PrintFullTable(removedUsers);
					ConsoleHelper.Pause();
					break;
				}
				case "users.view.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToUserMenu);
					return;
				}
			}
		}
	}



	private static string? ViewUserMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("users.view");
		return MenuHelper.DisplayMenu("VIEW USER MENU", items);
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
				if (u.ShouldRemove) return false;
				var allRolesRenewable = (u.RoleName == LibraryUserRole.Member && canRenewMembers) ||
				                        (u.RoleName == LibraryUserRole.Librarian && canRenewLibrarians);
				var hasRenewableRole = u.RoleName is LibraryUserRole.Member or LibraryUserRole.Librarian;

				return allRolesRenewable && hasRenewableRole;
			}).ToList();

		if (renewableUsers.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NoRenewableUser);
			return;
		}

		var selectedUser = MenuHelper.SelectExisting(renewableUsers,
			list => MenuHelper.SelectUser(list, users => UserPrinter.PrintTableForRenewingMembership(users)),
			Messages.NotAvailableUser);
		if (selectedUser is null) return;

		var years = ConsoleHelper.ReadInt(Messages.EnterYearForRenewMembership,
			ValidationConstants.MinRenewMembershipYear, ValidationConstants.MaxRenewMembershipYear);
		if (years is null) return;

		var result = userManagementService.RenewMembership(selectedUser.Id, years.Value);
		ConsoleHelper.ShowResult(result);
	}


	private static void ChangePassword(UserManagementService userManagementService, ICurrentUserSession session,
		IAuthorizationService authorization, IMenuService menuService)
	{
		Console.Clear();
		var selectedItem = MenuHelper.DisplayMenu("CHANGE PASSWORD", menuService.GetChildren("users.change-password"));
		if (selectedItem is null) return;
		Guid targetUserId;
		var isOwn = false;

		switch (selectedItem)
		{
			case "users.change-password.own":
			{
				targetUserId = session.CurrentUser.Id;
				isOwn = true;
				break;
			}
			case "users.change-password.other":
			{
				if (!authorization.HasPermission(Permission.ChangePassword))
				{
					ConsoleHelper.ShowError(Messages.AccessDenied);
					return;
				}

				var user = MenuHelper.SelectExisting(userManagementService.GetAllUsers(),
					list => MenuHelper.SelectUser(list, users => UserPrinter.PrintTable(users)),
					Messages.NotAvailableUser);
				if (user is null) return;
				if (user.ShouldRemove)
				{
					ConsoleHelper.ShowError(Messages.UserFlaggedForRemoval);
					return;
				}

				targetUserId = user.Id;
				isOwn = targetUserId == session.CurrentUser.Id;
				break;
			}
			default:
				return;
		}

		string? currentPassword = null;
		if (isOwn)
		{
			currentPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "current"));
			if (currentPassword is null) return;
		}

		var newPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "new"));
		if (newPassword is null) return;

		var confirmPassword = ConsoleHelper.GetValidPassword(Messages.PasswordConfirmation);
		if (confirmPassword is null) return;

		if (newPassword != confirmPassword)
		{
			ConsoleHelper.ShowError(Messages.PasswordMatchedFailed);
			return;
		}

		var result = userManagementService.ChangePassword(targetUserId, currentPassword, newPassword, session);
		ConsoleHelper.ShowResult(result);
	}
}