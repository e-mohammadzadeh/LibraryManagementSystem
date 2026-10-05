using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookDto
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string ISBN { get; init; } = null!;
	public DateOnly PublishDate { get; init; }
	public Genre Genre { get; init; }
	public string Publisher { get; init; } = null!;
	public Language OriginalLanguage { get; init; }
	public int TotalCopies { get; init; }
	public int AvailableCopies { get; init; }
	public string? Description { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
	public Guid? UpdatedByUserId { get; init; }
}