namespace LibraryManagementSystem.Application.DTOs.Authors;

public class AuthorDto
{
	public Guid Id { get; init; }
	public string FirstName { get; init; } = null!;
	public string LastName { get; init; } = null!;
	public string NationalCode { get; init; } = null!;
	public string Email { get; init; } = null!;
	public string PhoneNumber { get; init; } = null!;
	public DateOnly BirthDate { get; init; }
	public string? Biography { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}