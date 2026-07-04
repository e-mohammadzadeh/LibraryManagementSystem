using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Printers;
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
					Console.Clear();
					RemoveMember(userManagementService);
					break;
				case 4:
					SearchMember(userManagementService);
					break;
				case 5:
				{
					Console.Clear();
					var desiredMember = SelectExistingMember(userManagementService);
					if (desiredMember is not null)
					{
						MemberPrinter.PrintDetails(desiredMember);
						ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
						Console.ReadKey(true);
					}

					break;
				}
				case 6:
				{
					Console.Clear();
					if (userManagementService.GetAllMembers().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableMember);
					else
						MemberPrinter.PrintTable(userManagementService.GetAllMembers());

					ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
					Console.ReadKey(true);
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to Main Menu...\n");
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


	private static void RemoveMember(UserManagementService userManagementService)
	{
		Console.WriteLine("============================ REMOVING MEMBER MENU ============================");
		var desiredMember = SelectExistingMember(userManagementService);
		if (desiredMember == null)
		{
			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
			return;
		}

		MemberPrinter.PrintDetails(desiredMember);
		var choice = ConsoleHelper.ReadYesNo(
			$"Are you sure you want to remove {desiredMember.FirstName} {desiredMember.LastName}");

		if (choice != true) return;

		var result = userManagementService.RemoveMember(desiredMember.Id);
		ConsoleHelper.ShowResult(result);
		ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
		Console.ReadKey(true);
	}


	private static void SearchMember(UserManagementService userManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("============================ SEARCHING MEMBER MENU ============================");
			var membersList = userManagementService.GetAllMembers();
			if (membersList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableMember);
				ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
				Console.ReadKey(true);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Name");
			Console.WriteLine("{0, -20}", "2. National Code");
			Console.WriteLine("{0, -20}", "3. Email");
			Console.WriteLine("{0, -20}", "4. Phone Number");
			Console.WriteLine("5. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt("Select a search field by entering its number", 1, 5);
			if (searchMenuChoice is null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchMembersAndDisplay(userManagementService, "Enter a name to search",
						member => $"{member.FirstName} {member.LastName}");

					break;
				}
				case 2:
				{
					SearchMembersAndDisplay(userManagementService, "Enter a national code to search",
						member => member.NationalCode);

					break;
				}
				case 3:
				{
					SearchMembersAndDisplay(userManagementService, "Enter an email to search", member => member.Email);
					break;
				}
				case 4:
				{
					SearchMembersAndDisplay(userManagementService, "Enter a phone number to search",
						member => member.PhoneNumber);

					break;
				}
				case 5:
				{
					ConsoleHelper.ShowInfo("Search cancelled. Returning to Member Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.ShowInfo(ValidationMessages.Press2Continue);
			Console.ReadKey(true);
		}
	}


	private static void SearchMembersAndDisplay(UserManagementService userManagementService, string prompt,
		Func<Member, string?> selector)
	{
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem == null) return;

		var result = userManagementService.SearchMember(searchItem, selector);

		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotMemberMatched);
			return;
		}

		MemberPrinter.PrintTable(result);
	}
}