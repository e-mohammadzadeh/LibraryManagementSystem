namespace LibraryManagementSystem.DTOs;

public class CreateMemberDto
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string NationalCode { get; set; }
	public required string Email { get; set; }
	public required string PhoneNumber { get; set; }
	public required DateOnly BirthDate { get; set; }
}