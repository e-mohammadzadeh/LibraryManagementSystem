using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public ISBN ISBN { get; set; } = null!;
	public List<BookAuthor> Authors { get; set; } = null!;
	public List<BookTranslator> Translators { get; set; } = null!;
	public DateOnly PublishDate { get; set; }
	public BookGenre Genre { get; set; } = null!;
	public string Publisher { get; set; } = string.Empty;
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }
}