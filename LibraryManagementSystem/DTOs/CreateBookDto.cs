using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.DTOs;

public class CreateBookDto
{
	public required string ISBN { get; set; }
	public required string BookName { get; set; }
	public required int AuthorId { get; set; }
	public required DateOnly PublishDate { get; set; }
	public required int TotalCopies { get; set; }
	public required int GenreId { get; set; }
	public string? Description { get; set; }
}