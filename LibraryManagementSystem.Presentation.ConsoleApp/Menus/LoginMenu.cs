using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoginMenu
{
	public static AuthUserDto? ShowLogin(AuthenticationService authenticationService)
	{
		while (true)
		{
			Console.WriteLine(new string('=', 35) + " LOGIN MENU " + new string('=', 35));

			Console.WriteLine("Please log in to access the Library Management System.");
			var email = ConsoleHelper.GetValidEmail(Messages.EnterEmailPrompt);
			if (email is null) return null;

			var password = ConsoleHelper.GetValidPassword(Messages.EnterPasswordPrompt);

			var result = authenticationService.Login(email, password);
			if (result is { Success: true, Data: not null })
			{
				ConsoleHelper.ShowSuccess(result.Message ?? Messages.LoginSuccess);
				ConsoleHelper.Pause();
				return result.Data;
			}
			ConsoleHelper.ShowError(result.Message ?? Messages.LoginFailed);
			ConsoleHelper.Pause();
		}
	}
}