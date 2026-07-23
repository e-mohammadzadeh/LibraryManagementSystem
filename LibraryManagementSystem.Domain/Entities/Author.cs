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
	public string? Biography { get; set; }


	internal void AddBookAuthor(BookAuthor bookAuthor)
	{
		if (bookAuthor is null) throw new ArgumentNullException(nameof(bookAuthor));

		if (_bookAuthors.Any(ba => ba.BookId == bookAuthor.BookId)) return;

		_bookAuthors.Add(bookAuthor);
	}


	internal void RemoveBookAuthor(BookAuthor bookAuthor)
	{
		if (bookAuthor is null) throw new ArgumentNullException(nameof(bookAuthor));

		var existing = _bookAuthors.FirstOrDefault(ba => ba.BookId == bookAuthor.BookId);

		if (existing is not null) _bookAuthors.Remove(existing);
	}


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate, string? biography)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
		Biography = biography ?? Biography;
	}


	public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
}