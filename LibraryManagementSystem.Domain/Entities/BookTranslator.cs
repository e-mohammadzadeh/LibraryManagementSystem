namespace LibraryManagementSystem.Domain.Entities;

public class BookTranslator
{
	public Guid BookId { get; set; }
	public Book Book { get; set; } = null!;
	public Guid TranslatorId { get; set; }
	public Translator Translator { get; set; } = null!;
	public string Language { get; set; } = string.Empty;
}