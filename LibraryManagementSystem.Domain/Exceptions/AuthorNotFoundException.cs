namespace LibraryManagementSystem.Domain.Exceptions;

public class AuthorNotFoundException : DomainException
{
	public AuthorNotFoundException(Guid authorId) : base($"Author with id '{authorId}' was not found.") { }
}