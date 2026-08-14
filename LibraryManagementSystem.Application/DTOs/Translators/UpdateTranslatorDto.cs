namespace LibraryManagementSystem.Application.DTOs.Translators;

public class UpdateTranslatorDto
{
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public string? NationalCode { get; init; }
	public string? Email { get; init; }
	public string? PhoneNumber { get; init; }
	public DateOnly? BirthDate { get; init; }
}