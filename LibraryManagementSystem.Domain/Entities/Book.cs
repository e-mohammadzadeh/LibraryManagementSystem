using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public ISBN ISBN { get; set; } = null!;
	public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
	public ICollection<BookTranslator> BookTranslators { get; set; } = new List<BookTranslator>();
	public DateOnly PublishDate { get; set; }
	public BookGenre Genre { get; set; } = null!;
	public string Publisher { get; set; } = string.Empty;
	public Language OriginalLanguage { get; set; }
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? UpdatedByUserId { get; set; }
	public bool IsRemoved { get; set; }
}