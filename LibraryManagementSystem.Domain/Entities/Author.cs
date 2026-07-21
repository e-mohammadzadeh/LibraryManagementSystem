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
	private readonly List<BookAuthor> _bookAuthors = [];
	public string? Biography { get; private set; }


	internal void AddBookAuthor(BookAuthor bookAuthor)
	{
		if (!_bookAuthors.Contains(bookAuthor)) _bookAuthors.Add(bookAuthor);
	}


	internal void RemoveBookAuthor(BookAuthor bookAuthor)
	{
		_bookAuthors.Remove(bookAuthor);
	}

	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate, string? biography)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
		Biography = biography ?? Biography;
	}


	public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
}