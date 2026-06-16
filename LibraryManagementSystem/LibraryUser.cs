namespace LibraryManagementSystem;

public class LibraryUser : Person
{
	public LibraryUser(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		LibraryUserId = _nextLibraryUserId++;
	}

	private static int _nextLibraryUserId;
	public int LibraryUserId { get; private set; }
	public LibraryUserRole Role { get; set; }
}