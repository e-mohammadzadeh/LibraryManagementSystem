namespace LibraryManagementSystem.Domain.Exceptions;

public class AuthorNotFoundException : DomainException
{
	public AuthorNotFoundException(Guid authorId) : base($"Author with Id '{authorId}' was not found.") { }
}