using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class UpdateBookDto
{
	public Guid BookId { get; init; }
	public string? BookName { get; init; }
	public string? ISBN { get; init; }
	public List<Guid>? AuthorIds { get; init; }
	public List<Guid>? TranslatorIds { get; init; }
	public DateOnly? PublishDate { get; init; }
	public Genre? Genre { get; init; }
	public required Language OriginalLanguage { get; init; }
	public string? Publisher { get; init; }
	public int? TotalCopies { get; init; }
	public string? Description { get; init; }
	public required Language TranslatedLanguage { get; init; }
}