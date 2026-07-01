using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Enums;
using System.Xml.Linq;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private readonly List<Book> _books = new();


	public ServiceResult<Book> AddBook(CreateBookDto dto)
	{
		if (IsDuplicateBookName(dto.BookName))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (IsExistIsbn(dto.ISBN))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		var genreName = (Genre)dto.GenreId;
		var newBook = new Book(dto.ISBN, dto.BookName, dto.Author, dto.PublishDate, dto.TotalCopies, genreName,
			dto.Description);

		_books.Add(newBook);
		dto.Author.Books.Add(newBook);

		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


	public bool IsExistIsbn(string isbn)
	{
		return _books.Any(book =>
			book.InternationalStandardBookNumber.Equals(isbn, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Book> GetAllBooks()
	{
		return _books.AsReadOnly();
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

		if (dto.BookName != null && _books.Any(b =>
			    b.BookId != bookId && b.BookName.Equals(dto.BookName, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (dto.ISBN != null && _books.Any(b =>
			    b.BookId != bookId &&
			    b.InternationalStandardBookNumber.Equals(dto.ISBN, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (dto.GenreId != null && !Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		if (dto.TotalCopies.HasValue && dto.TotalCopies <= 0)
			return ServiceResult<Book>.Fail(ValidationMessages.WrongTotalCopies);

		Genre? genreName = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genreName, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

		if (dto.Author != null && !book.ChangeAuthor(dto.Author))
			return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}


	private bool IsDuplicateBookName(string name)
	{
		return _books.Any(book => book.BookName.Equals(name, StringComparison.OrdinalIgnoreCase));
	}
}