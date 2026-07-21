namespace LibraryManagementSystem.Domain.Entities;

public class BookAuthor
{
	public BookAuthor(Book book, Author author)
	{
		Book = book ?? throw new ArgumentNullException(nameof(book));
		Author = author ?? throw new ArgumentNullException(nameof(author));

		BookId = book.BookId;
		AuthorId = author.Id;
	}
	public int BookId { get; private set; }
	public Book Book { get; private set; }
	public int AuthorId { get; private set; }
	public Author Author { get; private set; }
}