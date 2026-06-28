using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private readonly List<Book> _books = new();


	public ServiceResult<Book> AddBook(string isbn, string bookName, Author author, DateOnly publishDate,
		int totalCopies, int genreId, string? description)
	{
		if (IsExistIsbn(isbn))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), genreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		var genreName = (Genre)genreId;
		var newBook = new Book(isbn, bookName, author, publishDate, totalCopies, genreName, description);

		_books.Add(newBook);
		author.Books.Add(newBook);

		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


	public bool IsExistIsbn(string isbn)
	{
		return _books.Any(book => book.InternationalStandardBookNumber == isbn);
	}


	public IReadOnlyList<Book> GetAllBooks()
	{
		return _books;
	}


	private Book? FindBookById(int id)
	{
		return _books.FirstOrDefault(b => b.BookId == id);
	}


	public ServiceResult<Book> UpdateBook(int bookId, string? bookName, string? isbn, Author? author,
		DateOnly? publishDate,
		int? totalCopies, int? genreId, string? description)
	{
		var book = FindBookById(bookId);
		if (book is null)
			return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

		if (bookName != null)
		{
			if (_books.Any(b => b.BookId == bookId && b.BookName == book.BookName))
			{
				return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);
			}
		}

		if (isbn != null && _books.Any(b => b.BookId != bookId && b.InternationalStandardBookNumber == isbn))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (author != null)
		{
		}

		if (genreId != null && !Enum.IsDefined(typeof(Genre), genreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		var genreName = (Genre)genreId;

		var dif = 0;
		if (totalCopies != null)
		{
			var old = book.TotalCopies;
			dif = (int)(totalCopies - old);
		}

		book.Update(bookName, isbn, author, publishDate, genreName, totalCopies, dif, description);
		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}
}