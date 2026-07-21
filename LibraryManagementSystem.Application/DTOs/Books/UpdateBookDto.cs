
namespace LibraryManagementSystem.Application.DTOs.Books;

public class UpdateBookDto
{
	public string? BookName { get; set; }
	public string? ISBN { get; set; }
	public List<int>? AuthorIds { get; set; }
	public int? TranslatorId { get; set; }
	public DateOnly? PublishDate { get; set; }
	public int? GenreId { get; set; }
	public string? Publisher { get; set; }
	public int? TotalCopies { get; set; }
	public string? Description { get; set; }
}