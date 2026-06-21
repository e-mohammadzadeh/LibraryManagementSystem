namespace LibraryManagementSystem.Domain;

public class Author : Person
{
	public Author(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, string? biography) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		AuthorId = _nextAuthorId++;
		Biography = biography;
	}

	private static int _nextAuthorId;
	public int AuthorId { get; private set; }
	public string? Biography { get; set; }
	public List<Book> Books { get; init; } = new();
}