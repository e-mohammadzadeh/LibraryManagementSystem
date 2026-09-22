namespace LibraryManagementSystem.Infrastructure.DTOs.Users;

public class CreateUserDto
{
	public required string FirstName { get; init; }
	public required string LastName { get; init; }
	public required string NationalCode { get; init; }
	public required string Email { get; init; }
	public required string PhoneNumber { get; init; }
	public required DateOnly BirthDate { get; init; }
	public required IReadOnlyList<Guid> RoleIds { get; init; } = [];
	public required string? Password { get; init; }
}