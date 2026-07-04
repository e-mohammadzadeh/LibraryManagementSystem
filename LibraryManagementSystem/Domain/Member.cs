namespace LibraryManagementSystem.Domain;

public class Member : LibraryUser
{
	public Member(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, bool isActive, DateOnly memberShipStartDate, DateOnly memberShipEndDate) : base(firstName,
		lastName, nationalCode, email, phoneNumber, birthDate, isActive, memberShipStartDate, memberShipEndDate)
	{
		Id = ++_nextMemberId;
	}
	private static int _nextMemberId;

}