namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class CreateBookDto
{
	public required string ISBN { get; init; }
	public required string BookName { get; init; }
	public List<Guid> AuthorIds { get; init; } = [];
	public List<Guid> TranslatorIds { get; init; } = [];
	public required DateOnly PublishDate { get; init; }
	public required int TotalCopies { get; init; }
	public required int GenreId { get; init; }
	public required string Publisher { get; init; }
	public string? Description { get; init; }
}