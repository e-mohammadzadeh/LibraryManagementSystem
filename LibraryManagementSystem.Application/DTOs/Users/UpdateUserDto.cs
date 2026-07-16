namespace LibraryManagementSystem.Application.DTOs.Users;

public class UpdateUserDto
{
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? BirthDate { get; set; }
	public List<int> RoleIds { get; set; } = new();
}