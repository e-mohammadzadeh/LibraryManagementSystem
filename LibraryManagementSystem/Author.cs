namespace LibraryManagementSystem;

public class Author:Person
{
	public Author(string firstName, string lastName) : base(firstName, lastName)
	{
	}

	public int AuthorId { get; set; }
	public string? Biography { get; set; }
	public List<Book> Books { get; init; } = new();
}