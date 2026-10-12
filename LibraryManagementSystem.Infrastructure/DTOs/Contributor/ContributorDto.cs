using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Contributor;

public class ContributorDto
{
	public Guid Id { get; init; }
	public string FirstName { get; init; } = null!;
	public string LastName { get; init; } = null!;
	public string NationalCode { get; init; } = null!;
	public Email Email { get; init; } = null!;
	public PhoneNumber PhoneNumber { get; init; } = null!;
	public DateOnly BirthDate { get; init; }
	public string? Biography { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}