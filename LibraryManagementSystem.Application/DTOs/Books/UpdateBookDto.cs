namespace LibraryManagementSystem.Application.DTOs.Books;

public class UpdateBookDto
{
	public string? BookName { get; init; }
	public string? ISBN { get; init; }
	public List<int>? AuthorIds { get; init; }
	public List<int>? TranslatorIds { get; init; }
	public DateOnly? PublishDate { get; init; }
	public int? GenreId { get; init; }
	public string? Publisher { get; init; }
	public int? TotalCopies { get; init; }
	public string? Description { get; init; }
}