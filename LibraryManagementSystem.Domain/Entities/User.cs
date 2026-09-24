namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	public Guid RoleId { get; set; }
	public Role Role { get; set; }
	public bool IsActive { get; set; }
	public DateOnly MembershipStartDate { get; }
	public DateOnly MembershipExpiryDate { get; set; }
	public bool ShouldRemove { get; set; }
	public byte[]? PasswordHash { get; set; }
	public byte[]? PasswordSalt { get; set; }
	public DateTime? LastLoginDate { get; set; }
	public DateTime? PreviousLoginDate { get; set; }


	public User(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, Role role, DateOnly? membershipStartDate = null) : base(firstName, lastName,
		nationalCode, email, phoneNumber, birthDate)
	{
		Role = role ?? throw new ArgumentNullException(nameof(role), "A user must have a role.");
		RoleId = role.Id;
		IsActive = true;
		MembershipStartDate = membershipStartDate ?? DateOnly.FromDateTime(DateTime.Today);
		// Should set a suitable end date based on business logic
		MembershipExpiryDate = MembershipStartDate.AddYears(1);
		ShouldRemove = false;
	}
}