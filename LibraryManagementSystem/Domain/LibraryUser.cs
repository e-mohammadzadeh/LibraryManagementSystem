namespace LibraryManagementSystem.Domain;

public abstract class LibraryUser : Person
{
	public LibraryUser(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		IsActive = true;
		MembershipStartDate = DateOnly.FromDateTime(DateTime.Today);
		MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today);  // Should set a suitable end date based on business logic
	}


	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; private set; }
	public DateOnly MembershipExpiryDate { get; private set; }
}