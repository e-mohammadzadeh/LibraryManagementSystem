namespace LibraryManagementSystem.Application.DTOs.Books;

public class BookDto
{
	public int BookId { get; init; }
	public string BookName { get; init; } = null!;
	public string ISBN { get; init; } = null!;
	public string Authors { get; init; } = null!;
	public string Translators { get; init; } = string.Empty;
	public DateOnly PublishDate { get; init; }
	public string Genre { get; init; } = null!;
	public string Publisher { get; init; } = null!;
	public int TotalCopies { get; init; }
	public int AvailableCopies { get; init; }
	public string? Description { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}