using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string InternationalStandardBookNumber { get; set; } = string.Empty;
	public List<BookAuthor> BookAuthors { get; set; } = null!;
	public List<BookTranslator> BookTranslators { get; set; } = null!;
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	public string Publisher { get; set; } = string.Empty;
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }
}