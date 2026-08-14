namespace LibraryManagementSystem.Application.DTOs.Authors;

public class CreateAuthorDto
{
	public required string FirstName { get; init; }
	public required string LastName { get; init; }
	public required string NationalCode { get; init; }
	public required string Email { get; init; }
	public required string PhoneNumber { get; init; }
	public required DateOnly BirthDate { get; init; }
	public string? Biography { get; init; }
}