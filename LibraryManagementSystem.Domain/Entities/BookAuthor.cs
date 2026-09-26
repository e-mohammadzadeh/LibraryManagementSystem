namespace LibraryManagementSystem.Domain.Entities;

public class BookAuthor
{
	public Guid BookId { get; private set; }
	public Book Book { get; private set; } = null!;
	public Guid AuthorId { get; private set; }
	public Author Author { get; private set; } = null!;
}