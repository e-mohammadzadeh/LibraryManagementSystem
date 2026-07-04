using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Printers;

public class MemberPrinter
{
	public static void PrintDetails(Member member) 
	{
		Console.WriteLine("\nMember Details:");

		Console.WriteLine("{0, -20} [{1} {2}]", "Name:", member.FirstName, member.LastName);
		Console.WriteLine("{0, -20} [{1}]", "National Code:", member.NationalCode);
		Console.WriteLine("{0, -20} [{1}]", "Email:", member.Email);
		Console.WriteLine("{0, -20} [{1}]", "Phone Number:", member.PhoneNumber);
		Console.WriteLine("{0, -20} [{1}]", "Birth Date:", member.BirthDate);
		//Console.WriteLine("{0, -20} [{1} {2}]", "Books:", member.Books.Count, "associated books");
	}


	public static void PrintTable(IReadOnlyList<Member> members) {
		if (members.Count == 0)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableMember);
			return;
		}

		Console.WriteLine("{0,3} {1, 20} {2, 25}", "ID", "Member Name", "Email Address");
		Console.WriteLine("========================================================");

		foreach (var member in members)
		{
			var fullName = member.FirstName + " " + member.LastName;
			Console.WriteLine("{0,3} {1, 20} {2, 25}", member.Id, fullName, member.Email);
		}

		Console.WriteLine("========================================================");
	}
}