using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums.Sort;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class LoginMenu
{
	public static AuthUserDto? ShowLogin(AuthenticationService authenticationService)
	{
		var directionRows = new List<string[][]>
		{
			new string[][] { ["1"], ["Login"] },
			new string[][] { ["2"], ["Create Account"] },
			new string[][] { ["3"], ["Exit"] }
		};
		ConsoleTable.PrintTable("Sort Direction", directionRows);

		var choice = ConsoleHelper.ReadInt(Messages.SortDirectionQuestion, 1, 3);
		if (choice is null) return null;

		switch (choice.Value)
		{
			case 1:
				return PerformLogin(authenticationService);
			case 2:
				return RegisterMenu.Show(authenticationService);
			case 3:
				return null;
		}
	}



	private static AuthUserDto? PerformLogin(AuthenticationService authenticationService)
	{
		while (true)
		{
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