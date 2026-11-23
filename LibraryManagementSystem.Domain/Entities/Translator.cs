namespace LibraryManagementSystem.Domain.Entities;

public class Translator
{
	public Guid Id { get; set; }

	public Guid PersonId { get; set; }
	public Person Person { get; set; } = null!;

	public DateOnly? DeathDate { get; set; }
	public string? Biography { get; set; }

	public bool IsRemoved { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? UpdatedByUserId { get; set; }

	public ICollection<BookTranslator> BookTranslators { get; set; } = [];
}