using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
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


	public ServiceResult<Book> UpdateBook(int bookId, UpdateBookDto dto)
	{
		var book = FindBookById(bookId);
		if (book is null)
			return ServiceResult<Book>.Fail(ValidationMessages.NotAvailableBook);

		if (dto.BookName is null)
		{
			if (_books.Any(b => b.BookId != bookId && b.BookName == dto.BookName))
			{
				return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);
			}
		}

		if (dto.ISBN is null || _books.Any(b => b.BookId != bookId && b.InternationalStandardBookNumber == dto.ISBN))
		{
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);
		}

		if (dto.GenreId is null || !Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		var genreName = (Genre)dto.GenreId;

		if (dto.Author != null)
		{
			if (book.Author.AuthorId == dto.Author.AuthorId)
				return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByAuthor);

			var oldAuthor = book.Author;
			oldAuthor.Books.Remove(book);
			book.Author = dto.Author;
			dto.Author.Books.Add(book);
		}
		else
			return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

		return book.Update(dto.BookName, dto.ISBN, dto.Author, dto.PublishDate, genreName, dto.TotalCopies,
			dto.Description)
			? ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully)
			: ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);
	}
}