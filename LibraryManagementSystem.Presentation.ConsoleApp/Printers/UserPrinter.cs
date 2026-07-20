using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class UserPrinter
{
	public static void PrintDetails(User user)
	{
		Console.WriteLine("\nUser Details:");

		Console.WriteLine("{0, -20} [{1} {2}]", "Name:", user.FirstName, user.LastName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", user.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", user.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", user.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", user.BirthDate);
		Console.WriteLine("{0, -20} [{1}]", "Role:", string.Join(", ", user.UserRoles.Select(ur => ur.Role.Name)));
		//Console.WriteLine("{0, -20} [{1} {2}]", "Books:", user.Books.Count, "associated books");
	}


	public static void PrintTable(IReadOnlyList<User> users)
	{
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableUser);
			return;
		}

		Console.WriteLine("{0,-3} {1, -30} {2, -12} {3, -40} {4, -15} {5, -12} {6, -9}", "ID", "User Name", "National Code",
			"Email Address", "Phone Number", "Birth Date", "Role");

		Console.WriteLine(new string('=', 160));

		foreach (var user in users)
		{
			var fullName = user.FirstName + " " + user.LastName;
			Console.WriteLine("{0,-3} {1, -30} {2, -12} {3, -40} {4, -15} {5, -12} {6, -9}", user.Id, fullName,
				user.NationalCode, user.Email, user.PhoneNumber, user.BirthDate,
				string.Join(", ", user.UserRoles.Select(ur => ur.Role.Name)));
		}

		Console.WriteLine(new string('=', 160));
	}
}