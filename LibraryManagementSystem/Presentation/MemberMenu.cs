using LibraryManagementSystem.Common;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public class MemberMenu
{
	public static void MemberMenuController(UserManagementService userManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			switch (MemberMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddMember(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 2:
					// Edit Member
					break;
				case 3:
					// Remove Member
					break;
				case 4:
					// Search Member
					break;
				case 5:
					// View Member Details
					break;
				case 6:
					// View All Members
					break;
				case 7:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int MemberMenuList()
	{
		while (true)
		{
			Console.WriteLine("============================ MEMBER MENU ============================");
			Console.WriteLine("1. Register Member");
			Console.WriteLine("2. Edit Member");
			Console.WriteLine("3. Remove Member");
			Console.WriteLine("4. Search Member");
			Console.WriteLine("5. View Member Details");
			Console.WriteLine("6. View All Members");
			Console.WriteLine("7. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddMember(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ ADDING MEMBER MENU ============================");
		var memberDto = PromptForMemberDto();
		if (memberDto is null) return;

		var result = userManagementService.AddMember(memberDto);
		ConsoleHelper.ShowResult(result);
	}


	public static CreateMemberDto? PromptForMemberDto() {
		var firstName = ConsoleHelper.GetValidName("Enter member's first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (firstName == null)
			return null;

		var lastName = ConsoleHelper.GetValidName("Enter member's last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);

		if (lastName == null)
			return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode("Enter member's national code");
		if (nationalCode == null)
			return null;

		var email = ConsoleHelper.GetValidEmail("Enter member's email");
		if (email == null)
			return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter member's phone number");
		if (phoneNumber == null)
			return null;

		var birthDate = ConsoleHelper.GetValidBirthDate("Enter member's birth date");
		if (birthDate == null)
			return null;


		return new CreateMemberDto()
		{
			FirstName = firstName,
			LastName = lastName,
			NationalCode = nationalCode,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = birthDate.Value
		};
	}
}