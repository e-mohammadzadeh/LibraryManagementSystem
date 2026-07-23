namespace LibraryManagementSystem.Domain.Entities;

public class BookTranslator
{
	public BookTranslator(Book book, Translator translator)
	{
		Book = book ?? throw new ArgumentNullException(nameof(book));
		Translator = translator ?? throw new ArgumentNullException(nameof(translator));

		BookId = book.BookId;
		TranslatorId = translator.Id;
	}


	private BookTranslator() { }

	public int BookId { get; private set; }
	public Book Book { get; private set; } = null!;
	public int TranslatorId { get; private set; }
	public Translator Translator { get; private set; } = null!;
}