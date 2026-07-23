namespace LibraryManagementSystem.Application.DTOs.Books;

public class CreateBookDto
{
	public required string ISBN { get; init; }
	public required string BookName { get; init; }
	public List<int> AuthorIds { get; set; } = [];
	public List<int>? TranslatorId { get; set; } = [];
	public required DateOnly PublishDate { get; init; }
	public required int TotalCopies { get; init; }
	public required int GenreId { get; init; }
	public required string Publisher { get; set; }
	public string? Description { get; init; }
}