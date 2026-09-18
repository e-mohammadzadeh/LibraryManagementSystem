namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookSummaryDto
{
	public Guid BookId { get; init; }
	public string BookName { get; init; } = null!;
	public string ISBN { get; init; } = null!;
	public int AvailableCopies { get; init; }
}