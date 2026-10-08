using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Users;

public class UserDto
{
	public Guid Id { get; init; }
	public string? FirstName { get; init; } = null!;
	public string? LastName { get; init; } = null!;
	public string? FullName => $"{FirstName} {LastName}";
	public string? NationalCode { get; init; } = null!;
	public Email? Email { get; init; } = null!;
	public PhoneNumber? PhoneNumber { get; init; } = null!;
	public DateOnly? BirthDate { get; init; }
	public Guid? RoleId { get; init; }
	public DateOnly? MembershipStartDate { get; init; }
	public DateOnly? MembershipExpiryDate { get; init; }
	public string? Password { get; init; }
	public bool? ShouldRemove { get; init; }
	public bool? IsActive { get; init; }
	public bool? IsRemoved { get; init; }
	public DateTime? CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
	public DateTime? LastLoginDate { get; init; }
}