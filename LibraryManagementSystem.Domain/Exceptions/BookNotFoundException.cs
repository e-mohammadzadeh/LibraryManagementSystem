namespace LibraryManagementSystem.Domain.Exceptions;

public class BookNotFoundException : DomainException
{
	public BookNotFoundException(Guid authorId) : base($"Author with Id '{authorId}' was not found.") { }

}