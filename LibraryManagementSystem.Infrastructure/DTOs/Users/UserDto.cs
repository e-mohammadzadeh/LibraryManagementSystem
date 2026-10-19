using LibraryManagementSystem.Infrastructure.DTOs.Role;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Users;

public class UserDto
{
	public Guid Id { get; init; }
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string FullName => $"{FirstName} {LastName}";
	public string NationalCode { get; set; } = null!;
	public Email Email { get; set; } = null!;
	public PhoneNumber PhoneNumber { get; set; } = null!;
	public DateOnly BirthDate { get; set; }
	public RoleDto Role { get; set; } = null!;
	public DateOnly MembershipStartDate { get; init; }
	public DateOnly MembershipExpiryDate { get; init; }
	public string Password { get; init; } = null!;
	public bool ShouldRemove { get; init; }
	public bool IsActive { get; init; }
	public bool IsRemoved { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
	public DateTime? LastLoginDate { get; init; }
}