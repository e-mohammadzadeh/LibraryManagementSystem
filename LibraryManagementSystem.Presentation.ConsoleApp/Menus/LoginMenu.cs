using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoginMenu
{
	public static AuthUserDto? ShowLoginMenu(AuthenticationService authenticationService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 35) + " LOGIN MENU " + new string('=', 35));
			Console.WriteLine("1. Login");
			Console.WriteLine("2. Create Account");
			Console.WriteLine("3. Exit");
			Console.WriteLine(new string('=', 82));

			var choice = ConsoleHelper.ReadInt(Messages.MainMenuQuestion, 1, 3, allowCancel: false);
			if (choice is null) return null;

			switch (choice.Value)
			{
				case 1:
					var loggedInUser = PerformLogin(authenticationService);
					if (loggedInUser is not null) return loggedInUser;
					break;
				case 2:
					var registeredUser = RegisterMenu.Show(authenticationService);
					if (registeredUser is not null) return registeredUser;
					break;
				case 3:
					ConsoleHelper.ShowInfo(Messages.ExitingProgram);
					return null;
			}
		}
	}



	private static AuthUserDto? PerformLogin(AuthenticationService authenticationService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 35) + " LOGIN MENU " + new string('=', 35));

			Console.WriteLine("Please log in to access the Library Management System.\n");
			var email = ConsoleHelper.GetValidEmail(Messages.EnterEmailPrompt);
			if (email is null) return null;

			var password =
				ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "").Replace("  ", " "));
			if (password == null) return null;
			var result = authenticationService.Login(email, password);
			if (result is { Success: true, Data: not null })
			{
				ConsoleHelper.ShowSuccess(result.Data.LastLoginDate is not null
					? $"\nWelcome back, {result.Data.FullName}! Your last login was on {result.Data.LastLoginDate.Value.ToLocalTime():f}."
					: $"\nWelcome, {result.Data.FullName}! This is your first login.");
				ConsoleHelper.Pause();
				return result.Data;
			}

			ConsoleHelper.ShowError(result.Message ?? Messages.LoginFailed);
		}
	}
}