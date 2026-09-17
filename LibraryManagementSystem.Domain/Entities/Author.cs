namespace LibraryManagementSystem.Domain.Entities;

public class Author : Person
{
	public Author(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, string? biography) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Biography = biography;
	}
	public string? Biography { get; private set; }


	//private static int _nextAuthorId;
	//private readonly List<BookAuthor> _bookAuthors = [];


	//internal void AddBookAuthor(BookAuthor bookAuthor)
	//{
	//	ArgumentNullException.ThrowIfNull(bookAuthor);
	//	if (_bookAuthors.Any(ba => ba.Id == bookAuthor.Id)) return;
	//	_bookAuthors.Add(bookAuthor);
	//	MarkAsUpdated();
	//}


	//internal void RemoveBookAuthor(BookAuthor bookAuthor)
	//{
	//	ArgumentNullException.ThrowIfNull(bookAuthor);
	//	var existing = _bookAuthors.FirstOrDefault(ba => ba.Id == bookAuthor.Id);
	//	if (existing is not null) _bookAuthors.Remove(existing);
	//	MarkAsUpdated();
	//}


	//public void DeleteAuthor() {
	//	IsRemoved = true;
	//	MarkAsUpdated();
	//}


	//public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
	//	DateOnly? birthDate, string? biography)
	//{
	//	UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
	//	Biography = biography ?? Biography;
	//}


	//public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
}