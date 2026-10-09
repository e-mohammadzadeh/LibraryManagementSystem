namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IBookManagementService
{
	void AssignAuthorsToBook(Book book, IEnumerable<Author> authors);
	void BorrowCopy(Guid bookId);
	void ReturnCopy(Guid bookId);
}