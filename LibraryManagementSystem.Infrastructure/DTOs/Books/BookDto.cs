using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookDto
{
	public Guid Id { get; init; }
	public string Title { get; set; } = null!;
	public ISBN ISBN { get; set; } = null!;
	public IReadOnlyList<ContributorDto> Authors { get; set; } = [];
	public IReadOnlyList<ContributorDto> Translators { get; set; } = [];
	public DateOnly PublishDate { get; set; }
	public BookGenre Genre { get; set; } = null!;
	public string Publisher { get; set; } = null!;
	public Language OriginalLanguage { get; set; }
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; init; }
	public string? Description { get; set; }
	public Language TranslatedLanguage { get; set; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
	public Guid UpdatedByUserId { get; init; }
}