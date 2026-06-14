namespace LibraryManagementSystem;

public class Author:Person
{
	public Author()
	{
		Books = new List<Book>();
	}
	public string? Biograph { get; set; }
	public List<Book> Books { get; set; }
}