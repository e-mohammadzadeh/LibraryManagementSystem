namespace LibraryManagementSystem.Application.DTOs.Books;

public class BookSummaryDto
{
	public int BookId { get; init; }
	public string BookName { get; init; } = null!;
	public string ISBN { get; init; } = null!;
}