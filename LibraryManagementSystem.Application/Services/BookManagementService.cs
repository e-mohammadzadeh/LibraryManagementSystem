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
	private readonly ILoanRepository _loanRepository;


	public BookManagementService(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository, ILoanRepository loanRepository)
	{
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_bookRepository = bookRepository;
		_loanRepository = loanRepository;
	}


	public ServiceResult<Book> AddBook(CreateBookDto dto)
	{
		if (_bookRepository.ExistsByName(dto.BookName))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (_bookRepository.ExistsByISBN(dto.ISBN))
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<Book>.Fail(ValidationMessages.InvalidGenre);
		var genre = (Genre)dto.GenreId;

		var authorIds = dto.AuthorIds.Distinct().ToList();
		if (authorIds.Count is 0) return ServiceResult<Book>.Fail(ValidationMessages.BookRequiresAtLeastOneAuthor);

		if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateAuthor);

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds)
		{
			var author = _authorRepository.FindById(authorId);
			if (author is null)
				return ServiceResult<Book>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, authorId));
			authors.Add(author);
		}

		Translator? translator = null;
		if (dto.TranslatorId.HasValue)
		{
			translator = _translatorRepository.FindById(dto.TranslatorId.Value);
			if (translator is null) return ServiceResult<Book>.Fail(ValidationMessages.BookAddFailed);
		}

		var newBook = new Book(dto.ISBN, dto.BookName, authors, translator, dto.PublishDate, dto.TotalCopies, genre,
			dto.Publisher, dto.Description);

		_bookRepository.Add(newBook);
		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


	public IReadOnlyList<Book> GetAllBooks() { return _bookRepository.GetAll(); }


	private Book? FindBookById(int id) { return _bookRepository.FindById(id); }


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

		Genre? genre = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genre, dto.Publisher, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<Book>.Fail(ValidationMessages.TotalCopiesUpdateInvalid);

		if (dto.AuthorIds is not null && dto.AuthorIds.Count > 0)
		{
			var newAuthors = new List<Author>();
			foreach (var id in dto.AuthorIds.Distinct())
			{
				var author = _authorRepository.FindById(id);
				if (author is null)
					return ServiceResult<Book>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, id));
				newAuthors.Add(author);
			}

			foreach (var oldAuthorLink in book.BookAuthors.ToList()) book.RemoveAuthor(oldAuthorLink.AuthorId);
			foreach (var author in newAuthors) book.AddAuthor(author);
		}

		if (dto.TranslatorId.HasValue)
		{
			var translator = _translatorRepository.FindById(dto.TranslatorId.Value);
			if (translator is null) return ServiceResult<Book>.Fail(ValidationMessages.NotTranslatorMatched);
			book.ChangeTranslator(translator);
		}

		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}


	public ServiceResult<Book> RemoveBook(int bookId)
	{
		var book = FindBookById(bookId);
		if (book is null) return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetActiveLoansByBook(bookId);
		if (activeLoans.Count > 0 || !book.CanBeRemoved())
			return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailedBorrowed);

		foreach (var authorLink in book.BookAuthors.ToList()) book.RemoveAuthor(authorLink.AuthorId);

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