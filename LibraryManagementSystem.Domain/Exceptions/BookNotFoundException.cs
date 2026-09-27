namespace LibraryManagementSystem.Domain.Exceptions;

public class BookNotFoundException : DomainException
{
	public BookNotFoundException(Guid authorId) : base($"Book with id '{authorId}' was not found.") { }

}