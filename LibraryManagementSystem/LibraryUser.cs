namespace LibraryManagementSystem;

public class LibraryUser : Person
{
	public LibraryUser(string firstName, string lastName) : base(firstName, lastName)
	{
	}

	public int LibraryUserId { get; set; }
	public LibraryUserRole Role { get; set; }
}