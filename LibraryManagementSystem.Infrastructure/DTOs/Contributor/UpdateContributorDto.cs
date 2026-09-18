namespace LibraryManagementSystem.Infrastructure.DTOs.Contributor;

public class UpdateContributorDto
{
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public string? NationalCode { get; init; }
	public string? Email { get; init; }
	public string? PhoneNumber { get; init; }
	public DateOnly? BirthDate { get; init; }
	public string? Biography { get; init; }
}