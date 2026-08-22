using System.Text;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class UserPrinter
{
	public static void PrintDetails(UserDto user, string title="User Details")
	{
		Console.Clear();
		Console.OutputEncoding = Encoding.UTF8;

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


	public static void PrintTable(IReadOnlyList<UserDto> users)
	{
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableUser);
			return;
		}

		Console.Clear();
		Console.WriteLine("\n{0,-3} {1, -30} {2, -12} {3, -40} {4, -15} {5, -9}", "ID", "User Name", "National Code",
			"Email Address", "Phone Number", "Role");

		Console.WriteLine(new string('=', 130));

		foreach (var user in users)
			Console.WriteLine("{0,-3} {1, -30} {2, -12} {3, -40} {4, -15} {5, -9}", user.Id, user.FullName,
				user.NationalCode, user.Email, user.PhoneNumber, string.Join(", ", user.Roles));


		Console.WriteLine(new string('=', 130));
	}
}