namespace LibraryManagementSystem.Domain.Entities;

public class Translator: Contributor 
{
	public ICollection<BookTranslator> BookTranslators { get; set; } = [];
}