using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private readonly List<Book> _Books = new();


	public ServiceResult<Book> AddBook(string isbn, string bookName, Author author, DateOnly publishDate,
		int totalCopies, int genreId, string? description)
	{
		if (IsExistIsbn(isbn))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), genreId))
			return ServiceResult<Book>.Fail("Invalid genre.");

		var genreName = (Genre)(genreId - 1);
		var newBook = new Book(isbn, bookName, author, publishDate, totalCopies, genreName, description);

		_Books.Add(newBook);
		author.Books.Add(newBook);

		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


	private bool IsExistIsbn(string isbn)
	{
		return _Books.Any(book => book.InternationalStandardBookNumber == isbn);
	}


	public IReadOnlyList<Book> GetAllBooks()
	{
		return _Books;
	}


	private Book? FindBookById(int id)
	{
		return _Books.FirstOrDefault(b => b.BookId == id);
	}


	public ServiceResult<Book> UpdateBook(int bookId, string? bookName, string? isbn, DateOnly? publishDate,
		int? totalCopies, int? genreId, string? description)
	{
		var book = FindBookById(bookId);
		if (book is null)
			return ServiceResult<Book>.Fail(ValidationMessages.AuthorUpdateFailed);

		if (isbn != null && IsExistIsbn(isbn) && isbn != book.InternationalStandardBookNumber)
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (genreId != null && !Enum.IsDefined(typeof(Genre), genreId))
			return ServiceResult<Book>.Fail("Invalid genre.");

		book.BookName = bookName ?? book.BookName;
		book.InternationalStandardBookNumber = isbn ?? book.InternationalStandardBookNumber;
		book.PublishDate = publishDate ?? book.PublishDate;
		book.Genre = genreId != null ? (Genre)(genreId.Value - 1) : book.Genre;
		book.Description = description ?? book.Description;
		if (totalCopies != null)
		{
			if (totalCopies < 0)
				return ServiceResult<Book>.Fail("Total copies cannot be negative.");

			book.TotalCopies = totalCopies.Value;
			book.AvailableCopies = Math.Min(book.AvailableCopies, totalCopies.Value);
		}

		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}
}