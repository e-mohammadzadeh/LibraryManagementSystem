using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Contributor;

public class ContributorDto
{
	public Guid Id { get; init; }
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string NationalCode { get; set; } = null!;
	public Email Email { get; set; } = null!;
	public PhoneNumber PhoneNumber { get; set; } = null!;
	public DateOnly BirthDate { get; set; }
	public string? Biography { get; set; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}