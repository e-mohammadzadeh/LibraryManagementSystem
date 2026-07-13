namespace LibraryManagementSystem.Domain.Entities;

public class Author : Person
{
	public Author(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, string? biography) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextAuthorId;
		Biography = biography;
	}


	private static int _nextAuthorId;
	public string? Biography { get; set; }
	public List<Book> Books { get; init; } = new();


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate, string? biography)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
		Biography = biography ?? Biography;
	}
}