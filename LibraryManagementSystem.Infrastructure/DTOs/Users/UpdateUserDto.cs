namespace LibraryManagementSystem.Infrastructure.DTOs.Users;

public class UpdateUserDto
{
	public Guid Id { get; init; }
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public string? NationalCode { get; init; }
	public string? Email { get; init; }
	public string? PhoneNumber { get; init; }
	public DateOnly? BirthDate { get; init; }
	public Guid? RoleId { get; init; }
}