namespace LibraryManagementSystem.Application.DTOs.Authors;

public class CreateAuthorDto
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string NationalCode { get; set; }
	public required string Email { get; set; }
	public required string PhoneNumber { get; set; }
	public required DateOnly BirthDate { get; set; }
	public string? Biography { get; set; }
}