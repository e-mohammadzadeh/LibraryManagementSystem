namespace LibraryManagementSystem.Domain.Entities;

public class BookTranslator
{
	public BookTranslator(Book book, Translator translator)
	{
		Book = book ?? throw new ArgumentNullException(nameof(book));
		Translator = translator ?? throw new ArgumentNullException(nameof(translator));

		BookId = book.Id;
		TranslatorId = translator.Id;
	}


	public Guid BookId { get; private set; }
	public Book Book { get; private set; }
	public Guid TranslatorId { get; private set; }
	public Translator Translator { get; private set; }
	public string Language { get; private set; }
}