namespace LibraryManagementSystem.Domain.Entities;

public class Translator : Person
{
	public Translator(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextTranslatorId;
	}


	private static int _nextTranslatorId;
	private readonly List<BookTranslator> _bookTranslators = [];


	internal void AddBookTranslator(BookTranslator bookTranslator)
	{
		if (_bookTranslators.Any(bt => bt.BookId == bookTranslator.BookId)) return;
		_bookTranslators.Add(bookTranslator);
		MarkAsUpdated();
	}


	internal void RemoveBookTranslator(BookTranslator bookTranslator)
	{
		var existing = _bookTranslators.FirstOrDefault(bt => bt.BookId == bookTranslator.BookId);
		if (existing is not null) _bookTranslators.Remove(existing);
		MarkAsUpdated();
	}


	public IReadOnlyList<BookTranslator> BookTranslators => _bookTranslators.AsReadOnly();


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
	}
}