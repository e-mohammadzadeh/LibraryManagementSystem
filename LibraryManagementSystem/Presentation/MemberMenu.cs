using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
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
				{
					Console.Clear();
					EditMember(userManagementService);
					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
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


	private static CreateMemberDto? PromptForMemberDto()
	{
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



	private static void EditMember(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ EDITING MEMBER MENU ============================");
		var desiredMember = SelectExistingMember(userManagementService);
		if (desiredMember == null) return;

		while (true)
		{
			Console.WriteLine("{0, -20} [{1}]", "1. First Name", desiredMember.FirstName);
			Console.WriteLine("{0, -20} [{1}]", "2. Last Name", desiredMember.LastName);
			Console.WriteLine("{0, -20} [{1}]", "3. National Code", desiredMember.NationalCode);
			Console.WriteLine("{0, -20} [{1}]", "4. Email", desiredMember.Email);
			Console.WriteLine("{0, -20} [{1}]", "5. Phone Number", desiredMember.PhoneNumber);
			Console.WriteLine("{0, -20} [{1}]", "6. Birth Date", desiredMember.BirthDate);
			Console.WriteLine("7. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt("Enter the number of the field you wish to edit", 1, 8);
			if (editMenuChoice == null)
				return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var memberNewFirstName = ConsoleHelper.GetValidName("Enter new first name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredMember.Id, memberNewFirstName,
						v => new UpdateMemberDto { FirstName = v });

					break;
				}
				case 2:
				{
					var memberNewLastName = ConsoleHelper.GetValidName("Enter new last name",
						ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

					PerformUpdate(userManagementService, desiredMember.Id, memberNewLastName,
						v => new UpdateMemberDto { LastName = v });

					break;
				}
				case 3:
				{
					var memberNewNationalCode = ConsoleHelper.GetValidNationalCode("Enter new national code");
					PerformUpdate(userManagementService, desiredMember.Id, memberNewNationalCode,
						v => new UpdateMemberDto { NationalCode = v });

					break;
				}
				case 4:
				{
					var memberNewEmail = ConsoleHelper.GetValidEmail("Enter new email");
					PerformUpdate(userManagementService, desiredMember.Id, memberNewEmail,
						v => new UpdateMemberDto { Email = v });

					break;
				}
				case 5:
				{
					var memberNewPhoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter new phone number");
					PerformUpdate(userManagementService, desiredMember.Id, memberNewPhoneNumber,
						v => new UpdateMemberDto { PhoneNumber = v });

					break;
				}
				case 6:
				{
					var memberNewBirthDate = ConsoleHelper.GetValidBirthDate("Enter new birth date");
					PerformUpdate(userManagementService, desiredMember.Id, memberNewBirthDate,
						v => new UpdateMemberDto { BirthDate = v });

					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Edit cancelled. Returning to Member Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo("Do you want to edit another field");
			if (choice != true) return;
			Console.Clear();
		}
	}


	private static Member? SelectExistingMember(UserManagementService userManagementService)
	{
		var member = userManagementService.GetAllMembers();
		if (member.Count is not 0) return MenuHelper.SelectMember(member);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableMember);
		return null;
	}


	private static void PerformUpdate<T>(UserManagementService userManagementService, int desiredMemberId, T? newValue,
		Func<T, UpdateMemberDto> buildDto)
	{
		if (newValue is null) return;
		var dto = buildDto(newValue);
		var result = userManagementService.UpdateMember(desiredMemberId, dto);
		ConsoleHelper.ShowResult(result);
	}
}