namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	public User(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextUserId;
		IsActive = true;
		MembershipStartDate = DateOnly.FromDateTime(DateTime.Today);
		MembershipExpiryDate = MembershipStartDate.AddYears(1);  // Should set a suitable end date based on business logic
	}

	private static int _nextUserId;
	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; private set; }
	public DateOnly MembershipExpiryDate { get; private set; }


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate) {
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
	}
}