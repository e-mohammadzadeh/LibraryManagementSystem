using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public class LoginMenu
{
	public static UserDto? LoginMenuOption(AuthenticationService authenticationService)
	{
		while (true)
		{
			Console.WriteLine(new string('=', 35) + " LOGIN MENU " + new string('=', 35));

			Console.WriteLine("Please log in to access the Library Management System.");
			var username = ConsoleHelper.GetValidEmail("Enter your email address");
			if (username is null) return null;

			var password = ConsoleHelper.GetValidPassword("Enter your password");
			if (password is null) return null;

			var result = authenticationService.Login(username, password);
			if (!result.Success)
			{
				ConsoleHelper.ShowError(result.Message);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return null;
			}

			ConsoleHelper.ShowSuccess(result.Message);
			return result.Data;
		}
	}
}