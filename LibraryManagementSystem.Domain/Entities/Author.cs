namespace LibraryManagementSystem.Domain.Entities;

public class Author : Contributor
{
	public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}