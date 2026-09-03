using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class UserPrinter
{
	public static void PrintDetails(UserDto user, string title = "User Details")
	{
		Console.Clear();
		var rows = new List<(string Label, string[] ValueLines)>
		{
			("ID", [user.Id.ToString()]),
			("Name", [user.FullName]),
			("National Code", [user.NationalCode]),
			("Email", [user.Email]),
			("Phone Number", [user.PhoneNumber]),
			("Birth Date", [user.BirthDate.ToString("yyyy-MM-dd")]),
			("Role", [string.Join(", ", user.Roles)]),
			("Active From", [user.MembershipStartDate.ToString("yyyy-MM-dd")]),
			("Active Until", [user.MembershipExpiryDate.ToString("yyyy-MM-dd")]),
			("Is Active", [user.IsActive ? "Yes" : "No"]),
			("Created At", [user.CreatedAt.ToString("yyyy-MM-dd HH:mm")]),
			("Updated At", [user.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"]),
			("Last Login", [user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never"]),
		};

		ConsoleTable.PrintKeyValueTable(title, rows, labelWidth: 18, valueWidth: 55);
	}


	public static void PrintTable(IReadOnlyList<UserDto> users, string title = "Users List")
	{
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableUser);
			return;
		}

		Console.Clear();
		var headers = new[]
		{
			"ID", "User Name", "Email", "Role", "Is Active",
		};

		var rows = users.Select(user => new[]
		{
			[user.Id.ToString()],
			ConsoleTable.WrapText(user.FullName, 20),
			ConsoleTable.WrapText(user.Email, 30),
			user.Roles.Count > 0 ? user.Roles.Select(r => r.ToString()).ToArray() : ["—"],
			[user.IsActive ? "Yes" : "No"],
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintTableForRenewingMembership(IReadOnlyList<UserDto> users, string title = "Users List") {
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableUser);
			return;
		}

		Console.Clear();
		var headers = new[]
		{
			"ID", "User Name", "Email", "Role", "Is Active", "Active From", "Active Until",
		};

		var rows = users.Select(user => new[]
		{
			[user.Id.ToString()],
			ConsoleTable.WrapText(user.FullName, 20),
			ConsoleTable.WrapText(user.Email, 30),
			user.Roles.Count > 0 ? user.Roles.Select(r => r.ToString()).ToArray() : ["—"],
			[user.IsActive ? "Yes" : "No"],
			[user.MembershipStartDate.ToString("yyyy-MM-dd")],
			[user.MembershipExpiryDate.ToString("yyyy-MM-dd")]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}


	public static void PrintFullTable(IReadOnlyList<UserDto> users, string title = "User Full Information")
	{
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableUser);
			return;
		}

		Console.Clear();
		var headers = new[]
		{
			"ID", "User Name", "National Code", "Email", "Phone", "Role", "Active From", "Active Until", "Is Active",
			"Should Remove", "Last Login", "Updated At"
		};

		var rows = users.Select(user => new[]
		{
			[user.Id.ToString()],
			ConsoleTable.WrapText(user.FullName, 20),
			[user.NationalCode],
			ConsoleTable.WrapText(user.Email, 30),
			[user.PhoneNumber],
			user.Roles.Count > 0 ? user.Roles.Select(r => r.ToString()).ToArray() : ["—"],
			[user.MembershipStartDate.ToString("yyyy-MM-dd")],
			[user.MembershipExpiryDate.ToString("yyyy-MM-dd")],
			[user.IsActive ? "Yes" : "No"],
			[user.ShouldRemove ? "Yes" : "No"],
			[user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never"],
			[user.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—"]
		}).ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}