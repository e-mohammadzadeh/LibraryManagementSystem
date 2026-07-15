using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.DTOs.Users;

public class CreateUserDto
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string NationalCode { get; set; }
	public required string Email { get; set; }
	public required string PhoneNumber { get; set; }
	public required DateOnly BirthDate { get; set; }
	public required List<int> RoleIds { get; set; } = new();
}