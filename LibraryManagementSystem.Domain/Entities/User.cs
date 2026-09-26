namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	public Guid RoleId { get; set; }
	public Role Role { get; set; } = null!;
	public bool IsActive { get; set; }
	public DateOnly MembershipStartDate { get; set; }
	public DateOnly MembershipExpiryDate { get; set; }
	public bool ShouldRemove { get; set; }
	public byte[]? PasswordHash { get; set; }
	public byte[]? PasswordSalt { get; set; }
	public DateTime? LastLoginDate { get; set; }
	public DateTime? PreviousLoginDate { get; set; }
}