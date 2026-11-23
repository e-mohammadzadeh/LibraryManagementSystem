namespace LibraryManagementSystem.Domain.Entities;

public class User
{
	public Guid Id { get; set; }

	public Guid PersonId { get; set; }
	public Person Person { get; set; } = null!;

	public Guid RoleId { get; set; }
	public Role Role { get; set; } = null!;

	public bool IsRemoved { get; set; }
	public bool IsActive { get; set; }

	public DateOnly MembershipStartDate { get; set; }
	public DateOnly MembershipExpiryDate { get; set; }
	public bool ShouldRemove { get; set; }

	public byte[]? PasswordHash { get; set; }
	public byte[]? PasswordSalt { get; set; }

	public DateTime? LastLoginDate { get; set; }
	public DateTime? PreviousLoginDate { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? UpdatedByUserId { get; set; }
}