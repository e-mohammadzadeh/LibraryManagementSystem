namespace LibraryManagementSystem.Domain.Entities;

public class Author : Person
{
	public string? Biography { get; set; }

	public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}