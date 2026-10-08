using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookDto
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public ISBN ISBN { get; init; } = null!;
	public List<Guid> AuthorIds { get; init; }
	public List<Guid> TranslatorIds { get; init; }
	public DateOnly PublishDate { get; init; }
	public BookGenre Genre { get; init; }
	public string Publisher { get; init; } = null!;
	public Language OriginalLanguage { get; init; }
	public int TotalCopies { get; init; }
	public int AvailableCopies { get; init; }
	public string? Description { get; init; }
	public Language? TranslatedLanguage { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
	public Guid? UpdatedByUserId { get; init; }
}