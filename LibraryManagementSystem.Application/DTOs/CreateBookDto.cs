namespace LibraryManagementSystem.DTOs;

public class CreateBookDto
{
	public required string ISBN { get; init; }
	public required string BookName { get; init; }
	public required int AuthorId { get; init; }
	public required DateOnly PublishDate { get; init; }
	public required int TotalCopies { get; init; }
	public required int GenreId { get; init; }
	public string? Description { get; init; }
}