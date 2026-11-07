namespace LibraryManagementSystem.Domain.Exceptions;

public class BookAuthorNotFoundException : DomainException
{
	public BookAuthorNotFoundException(Guid authorId, Guid bookId) : base(
		$"Author with id {authorId} and book with id {bookId} was not found.")
	{
	}
}