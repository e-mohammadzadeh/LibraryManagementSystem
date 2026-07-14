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
		//Console.WriteLine("{0, -20} [{1} {2}]", "Books:", user.Books.Count, "associated books");
	}


	public static void PrintTable(IReadOnlyList<User> users) {
		if (users.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableUser);
			return;
		}

		Console.WriteLine("{0,3} {1, 20} {2, 25}", "ID", "User Name", "Email Address");
		Console.WriteLine("========================================================");

		foreach (var user in users)
		{
			var fullName = user.FirstName + " " + user.LastName;
			Console.WriteLine("{0,3} {1, 20} {2, 25}", user.Id, fullName, user.Email);
		}

		Console.WriteLine("========================================================");
	}
}