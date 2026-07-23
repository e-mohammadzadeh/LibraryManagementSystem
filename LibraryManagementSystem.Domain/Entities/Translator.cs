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
		if (!_bookTranslators.Contains(bookTranslator)) _bookTranslators.Add(bookTranslator);
	}


	internal void RemoveBookTranslator(BookTranslator bookTranslator) { _bookTranslators.Remove(bookTranslator); }


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
	}
}