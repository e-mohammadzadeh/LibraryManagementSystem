using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private readonly List<Book> _books = new();
	private readonly UserManagementService _authorService;


	public BookManagementService(UserManagementService authorService)
	{
		_authorService = authorService;
	}


	public ServiceResult<Book> AddBook(CreateBookDto dto)
	{
		if (IsDuplicateBookName(dto.BookName))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (IsExistIsbn(dto.ISBN))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		var genreName = (Genre)dto.GenreId;
		var author = _authorService.FindAuthorById(dto.AuthorId);
		if (author == null)
			return ServiceResult<Book>.Fail(ValidationMessages.BookAddFailed);

		var newBook = new Book(dto.ISBN, dto.BookName, author, dto.PublishDate, dto.TotalCopies, genreName,
			dto.Description);

		_books.Add(newBook);
		author.Books.Add(newBook);

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
			return ServiceResult<Book>.Fail(
				"Cannot update total copies because it would result in negative available copies.");

		if (dto.AuthorId.HasValue)
		{
			var author = _authorService.FindAuthorById(dto.AuthorId.Value);
			if (author == null)
				return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

			book.ChangeAuthor(author);
		}

		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}


	private bool IsDuplicateBookName(string name)
	{
		return _books.Any(book => book.BookName.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public ServiceResult<Book> RemoveBook(int bookId)
	{
		var book = FindBookById(bookId);
		if (book is null)
			return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailed);

		if (!book.CanBeRemoved())
			return ServiceResult<Book>.Fail("Failed to remove Book. It is currently borrowed by a user.");

		//TODO	Missing loan integration: if book has active loans, it cannot be removed. This should be checked with the loan management service.
		book.RemoveFromCurrentAuthor();
		_books.Remove(book);
		return ServiceResult<Book>.Ok(book, ValidationMessages.BookRemovedSuccessfully);
	}


	public IReadOnlyList<Book> SearchBooks<T>(T? searchItem, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		if (searchItem is null)
			return new List<Book>();

		return _books.Where(book =>
		{
			var value = selector(book);
			return value is not null && comparer(searchItem, value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<Book> SearchBooks<T>(T? searchItem, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchItem.HasValue)
			return new List<Book>();

		return _books.Where(book =>
		{
			var value = selector(book);
			return value.HasValue && comparer(searchItem.Value, value.Value);
		}).ToList().AsReadOnly();
	}
}