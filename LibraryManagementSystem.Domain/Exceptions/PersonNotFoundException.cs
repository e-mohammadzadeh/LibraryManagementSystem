namespace LibraryManagementSystem.Domain.Exceptions;

public class PersonNotFoundException : DomainException
{
	public PersonNotFoundException(Guid id) : base($"Person with ID '{id}' was not found.") { }
}