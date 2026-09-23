namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	public Guid RoleId { get; set; }
	public Role Role { get; set; }
	public bool IsActive { get; set; }
	public DateOnly MembershipStartDate { get; }
	public DateOnly MembershipExpiryDate { get; private set; }
	public bool ShouldRemove { get; private set; }
	public byte[]? PasswordHash { get; private set; }
	public byte[]? PasswordSalt { get; private set; }
	public DateTime? LastLoginDate { get; private set; }
	private DateTime? PreviousLoginDate { get; set; }


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



	public void RenewMembership(int years = 1)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		var renewalBase = MembershipExpiryDate > today
			? MembershipExpiryDate // extend from current expiry if not yet expired
			: today; // restart from today if already expired

		MembershipExpiryDate = renewalBase.AddYears(years);
		if (!IsActive) IsActive = true;
		MarkAsUpdated();
	}


	public void FlagForRemoval()
	{
		ShouldRemove = true;
		MarkAsUpdated();
	}



	public void SetPasswordHash(byte[] passwordHash, byte[] passwordSalt)
	{
		if (passwordHash is null || passwordHash.Length == 0) throw new ArgumentNullException(nameof(passwordHash));
		if (passwordSalt is null || passwordSalt.Length == 0) throw new ArgumentNullException(nameof(passwordSalt));

		PasswordHash = passwordHash;
		PasswordSalt = passwordSalt;
	}


	public bool HasPassword() => PasswordHash is { Length: > 0 } && PasswordSalt is { Length: > 0 };


	public void UpdateLastLogin()
	{
		LastLoginDate = PreviousLoginDate;
		PreviousLoginDate = DateTime.Now;
	}


	public void UpdateLastLoginInLogout()
	{
		LastLoginDate = PreviousLoginDate;
		MarkAsUpdated();
	}
}