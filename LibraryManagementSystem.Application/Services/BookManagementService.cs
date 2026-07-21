using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class BookManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IBookRepository _bookRepository;


	public BookManagementService(IBookRepository bookRepository, ITranslatorRepository translatorRepository,
		IAuthorRepository authorRepository)
	{
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_bookRepository = bookRepository;
	}


	public ServiceResult<Book> AddBook(CreateBookDto dto)
	{
		if (_bookRepository.ExistsByName(dto.BookName))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (_bookRepository.ExistsByISBN(dto.ISBN))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);
		var genreName = (Genre)dto.GenreId;

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds)
		{
			var author = _authorRepository.FindById(authorId);
			if (author is null)
				return ServiceResult<Book>.Fail()
		}

		var author = _authorRepository.FindById(dto.AuthorId);
		if (author is null) return ServiceResult<Book>.Fail(ValidationMessages.BookAddFailed);

		var translator = _translatorRepository.FindById(dto.TranslatorId);
		if (translator is null) return ServiceResult<Book>.Fail(ValidationMessages.BookAddFailed);

		var newBook = new Book(dto.ISBN, dto.BookName, author, translator, dto.PublishDate, dto.TotalCopies, genreName,
			dto.Publisher,
			dto.Description);

		_bookRepository.Add(newBook);
		author.Books.Add(newBook);

		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


	public bool IsExistISBN(string isbn) { return _bookRepository.ExistsByISBN(isbn); }


	public IReadOnlyList<Book> GetAllBooks() { return _bookRepository.GetAll(); }


	public Book? FindBookById(int id) { return _bookRepository.FindById(id); }


	public ServiceResult<Book> UpdateBook(int bookId, UpdateBookDto dto)
	{
		var book = FindBookById(bookId);
		if (book is null) return ServiceResult<Book>.Fail(ValidationMessages.NotAvailableBook);

		if (dto.BookName != null && _bookRepository.ExistsByName(dto.BookName, bookId))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (dto.ISBN != null && _bookRepository.ExistsByISBN(dto.ISBN, bookId))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (dto.GenreId != null && !Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);

		if (dto.TotalCopies.HasValue && dto.TotalCopies.Value <= 0)
			return ServiceResult<Book>.Fail(ValidationMessages.WrongTotalCopies);

		Genre? genreName = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genreName, dto.Publisher, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<Book>.Fail(
				"Cannot update total copies because it would result in negative available copies.");

		if (dto.AuthorId.HasValue)
		{
			var author = _authorRepository.FindById(dto.AuthorId.Value);
			if (author == null) return ServiceResult<Book>.Fail(ValidationMessages.BookUpdateFailed);

			book.ChangeAuthor(author);
		}

		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}


	public ServiceResult<Book> RemoveBook(int bookId)
	{
		var book = FindBookById(bookId);
		if (book is null) return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailed);

		if (!book.CanBeRemoved())
			return ServiceResult<Book>.Fail("Failed to remove Book. It is currently borrowed by a user.");

		//TODO	Missing loan integration: if book has active loans, it cannot be removed. This should be checked with the loan management service.
		//book.RemoveFromCurrentAuthor();  // If ChangeAuthor is implemented correctly, this line is not needed.
		book.ChangeAuthor(null);
		_bookRepository.Remove(book);
		return ServiceResult<Book>.Ok(book, ValidationMessages.BookRemovedSuccessfully);
	}


	public IReadOnlyList<Book> SearchBooks<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		return _bookRepository.Search(searchTerm, selector, comparer);
	}


	public IReadOnlyList<Book> SearchBooks<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		return _bookRepository.Search(searchTerm, selector, comparer);
	}


	public IReadOnlyList<Book> GetAvailableBooks() { return _bookRepository.GetAvailableBooks(); }
}