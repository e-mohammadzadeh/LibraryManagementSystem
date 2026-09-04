using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class RegisterMenu
{
	public static AuthUserDto? Show(AuthenticationService authenticationService)
	{
		Console.Clear();
		Console.WriteLine(new string('=', 30) + " CREATE ACCOUNT " + new string('=', 30));

		var fields = PersonHelper.PromptForPersonFields("user");
		if (fields is null)
		{
			ConsoleHelper.ShowInfo(Messages.RegistrationCancelled);
			return null;
		}

		var newPassword = ConsoleHelper.GetValidPassword(string.Format(Messages.EnterPasswordPrompt, "new"));
		if (newPassword is null) return null;

		while (true)
		{
			var confirmPassword = ConsoleHelper.GetValidPassword(Messages.PasswordConfirmation);
			if (confirmPassword is null) return null;
			if (newPassword == confirmPassword) break;
			ConsoleHelper.ShowError(Messages.PasswordMatchedFailed);
		}

		var dto = new CreateUserDto
		{
			FirstName = fields.FirstName,
			LastName = fields.LastName,
			NationalCode = fields.NationalCode,
			Email = fields.Email,
			PhoneNumber = fields.PhoneNumber,
			BirthDate = fields.BirthDate,
			RoleIds = [1], // Force role to be Member
			Password = newPassword
		};

		var result = authenticationService.Register(dto);

		if (!result.Success || result.Data is null)
		{
			ConsoleHelper.ShowError(result.Message ?? Messages.UserRegistrationFailed);
			return null;
		}

		if (result.IsWarning)
			ConsoleHelper.ShowWarning(result.Message!);
		else
			ConsoleHelper.ShowSuccess(result.Message ?? Messages.UserRegistrationSuccessfully);

		ConsoleHelper.ShowSuccess(
			$"\nWelcome, {result.Data.FullName}! Your account was created and you are now logged in.");
		return result.Data;
	}
}