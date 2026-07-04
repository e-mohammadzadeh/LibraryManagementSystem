namespace LibraryManagementSystem.DTOs;

public class UpdateMemberDto
{
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? BirthDate { get; set; }
}