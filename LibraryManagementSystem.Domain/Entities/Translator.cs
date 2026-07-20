namespace LibraryManagementSystem.Domain.Entities;

public class Translator : Person
{
	public Translator(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextTranslatorId;
	}

	
	private static int _nextTranslatorId;
	public List<Book> Books { get; set; }


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate) 
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
	}
}