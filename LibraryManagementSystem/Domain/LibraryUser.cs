namespace LibraryManagementSystem.Domain;

public abstract class LibraryUser : Person
{
	public LibraryUser(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, bool isActive, DateOnly memberShipStartDate, DateOnly memberShipEndDate) : base(firstName,
		lastName, nationalCode, email, phoneNumber, birthDate)
	{
		IsActive = isActive;
		MembershipStartDate = memberShipStartDate;
		MembershipEndDate = memberShipEndDate;
	}


	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; private set; }
	public DateOnly MembershipEndDate { get; private set; }
}