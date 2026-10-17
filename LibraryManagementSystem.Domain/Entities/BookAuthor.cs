namespace LibraryManagementSystem.Domain.Entities;

public class BookAuthor
{
	public Guid BookId { get; set; }
	public virtual Book Book { get; set; } = null!;
	public Guid AuthorId { get; set; }
	public virtual Author Author { get; set; } = null!;
}