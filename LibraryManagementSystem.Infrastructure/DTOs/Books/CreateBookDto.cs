using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class CreateBookDto
{
	public required string Title { get; init; }
	public required string ISBN { get; init; }
	public List<Guid> AuthorIds { get; init; } = [];
	public List<Guid> TranslatorIds { get; init; } = [];
	public required DateOnly PublishDate { get; init; }
	public required Genre Genre { get; init; }
	public required string Publisher { get; init; }
	public required Language OriginalLanguage { get; init; }
	public required int TotalCopies { get; init; }
	public string? Description { get; init; }
	public Language? TranslatedLanguage { get; init; }
}