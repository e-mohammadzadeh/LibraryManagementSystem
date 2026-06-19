using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private List<Book> _listOfBooks = new();

	public ServiceResult<Book> AddBook(string isbn, string bookName, Author author, DateOnly publishDate,
		int totalCopies, int genreId, string? description)
	{
		if (IsExistIsbn(isbn))
			return ServiceResult<Book>.Fail("This ISBN is already exists. Please enter a new ISBN.");
		if (!Enum.IsDefined(typeof(Genre), genreId))
			return ServiceResult<Book>.Fail("Invalid genre.");

		var genreName = (Genre)(genreId - 1);
		var newBook = new Book(isbn, bookName, author, publishDate, totalCopies, genreName, description);

		_listOfBooks.Add(newBook);

		return ServiceResult<Book>.Ok(newBook);
	}


	private bool IsExistIsbn(string isbn)
	{
		return _listOfBooks.Any(book => book.InternationalStandardBookNumber == isbn);
	}
}