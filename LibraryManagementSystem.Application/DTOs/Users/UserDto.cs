namespace LibraryManagementSystem.Application.DTOs.Users;

public class UserDto
{
	public int Id { get; init; }
	public string FirstName { get; init; } = null!;
	public string LastName { get; init; } = null!;
	public string FullName => $"{FirstName} {LastName}";
	public string NationalCode { get; init; } = null!;
	public string Email { get; init; } = null!;
	public string PhoneNumber { get; init; } = null!;
	public DateOnly BirthDate { get; init; }
	public string Roles { get; init; } = null!;
	public DateOnly MembershipStartDate { get; init; }
	public DateOnly MembershipExpiryDate { get; init; }
	public bool IsActive { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}