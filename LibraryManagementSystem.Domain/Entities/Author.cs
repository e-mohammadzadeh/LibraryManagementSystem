namespace LibraryManagementSystem.Domain.Entities;

public class Author : Person
{
	public Author(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, string? biography) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Biography = biography;
	}
	public string? Biography { get; set; }
}