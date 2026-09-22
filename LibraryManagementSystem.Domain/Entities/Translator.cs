namespace LibraryManagementSystem.Domain.Entities;

public class Translator : Person
{
	public Translator(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, string? biography) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Biography = biography;
	}

	public string? Biography { get; set; }
}