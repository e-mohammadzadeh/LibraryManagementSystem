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

			Console.WriteLine("Please log in to access the Library Management System.\n");
			var email = ConsoleHelper.GetValidEmail(Messages.EnterEmailPrompt);
			if (email is null) return null;

			var password = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, ""));

			var result = authenticationService.Login(email, password);
			if (result is { Success: true, Data: not null })
			{
				ConsoleHelper.ShowInfo(result.Data.LastLoginDate is not null
					? $"Welcome back, {result.Data.FullName}! Your last login was on {result.Data.LastLoginDate.Value.ToLocalTime():f}."
					: $"Welcome, {result.Data.FullName}! This is your first login.");
				ConsoleHelper.Pause();
				return result.Data;
			}
			ConsoleHelper.ShowError(result.Message ?? Messages.LoginFailed);
		}
	}
}