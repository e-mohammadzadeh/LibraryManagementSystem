using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Translator;

namespace LibraryManagementSystem.Application.DTOs.Books;

public class BookDto
{
	public int BookId { get; init; }
	public string BookName { get; init; } = null!;
	public string ISBN { get; init; } = null!;
	public IReadOnlyList<AuthorDto> Authors { get; init; } = [];
	public IReadOnlyList<TranslatorDto> Translators { get; init; } = [];
	public DateOnly PublishDate { get; init; }
	public string Genre { get; init; } = null!;
	public string Publisher { get; init; } = null!;
	public int TotalCopies { get; init; }
	public int AvailableCopies { get; init; }
	public string? Description { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}