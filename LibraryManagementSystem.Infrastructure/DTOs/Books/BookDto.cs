using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookDto
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public ISBN ISBN { get; init; } = null!;
	public IReadOnlyList<ContributorDto> Authors { get; init; } = [];
	public IReadOnlyList<ContributorDto> Translators { get; init; } = [];
	public DateOnly PublishDate { get; init; }
	public BookGenre Genre { get; init; } = null!;
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