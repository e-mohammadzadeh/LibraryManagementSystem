namespace LibraryManagementSystem.Domain.Entities;

public class Translator : Person
{
	public string? Biography { get; set; }

	public ICollection<BookTranslator> BookTranslators { get; set; } = new List<BookTranslator>();
}