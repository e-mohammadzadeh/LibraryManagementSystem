namespace LibraryManagementSystem.Domain.Exceptions;

public class BookNotAvailableException : DomainException
{
	public BookNotAvailableException(string bookTitle) : base($"Book '{bookTitle}' is not available for borrowing.") { }
}