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

		if (dto.AuthorIds.Count is 0) return ServiceResult<Book>.Fail(ValidationMessages.BookRequiresAtLeastOneAuthor);

		if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
			return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateAuthor);

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds.ToList())
		{
			var author = _authorRepository.FindById(authorId);
			if (author is null)
				return ServiceResult<Book>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, authorId));
			authors.Add(author);
		}

		var translators = new List<Translator>();
		foreach (var translatorId in dto.TranslatorIds)
		{
			var translator = _translatorRepository.FindById(translatorId);
			if (translator is null)
				return ServiceResult<Book>.Fail(ValidationMessages.NotTranslatorMatched);
			translators.Add(translator);
		}

		var newBook = new Book(dto.ISBN, dto.BookName, authors, translators, dto.PublishDate, dto.TotalCopies, genre,
			dto.Publisher, dto.Description);

		_bookRepository.Add(newBook);
		return ServiceResult<Book>.Ok(newBook, ValidationMessages.BookAddedSuccessfully);
	}


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

		List<Author>? resolvedAuthors = null;
		if (dto.AuthorIds is not null)
		{
			if (dto.AuthorIds.Count == 0)
				return ServiceResult<Book>.Fail(ValidationMessages.BookRequiresAtLeastOneAuthor);

			if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
				return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateAuthor);


			resolvedAuthors = new List<Author>();
			foreach (var id in dto.AuthorIds.Distinct())
			{
				var author = _authorRepository.FindById(id);
				if (author is null)
					return ServiceResult<Book>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, id));
				resolvedAuthors.Add(author);
			}
		}

		List<Translator>? resolvedTranslators = null;
		if (dto.TranslatorId is not null)
		{
			if (dto.TranslatorId.Count != dto.TranslatorId.Distinct().Count())
				return ServiceResult<Book>.Fail(ValidationMessages.FailureDuplicateTranslator);

			resolvedTranslators = new List<Translator>();
			foreach (var translatorId in dto.TranslatorId)
			{
				var translator = _translatorRepository.FindById(translatorId);
				if (translator is null)
					return ServiceResult<Book>.Fail(string.Format(ValidationMessages.TranslatorNotFoundFormat,
						translatorId));
				resolvedTranslators.Add(translator);
			}
		}

		Genre? genre = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genre, dto.Publisher, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<Book>.Fail(ValidationMessages.TotalCopiesUpdateInvalid);

		if (resolvedAuthors is not null)
		{
			var currentAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList();
			foreach (var authorId in currentAuthorIds) book.RemoveAuthor(authorId);

			foreach (var author in resolvedAuthors) book.AddAuthor(author);
		}

		if (resolvedTranslators is not null)
		{
			var currentTranslatorIds = book.BookTranslators.Select(bt => bt.TranslatorId).ToList();
			foreach (var translatorId in currentTranslatorIds) book.RemoveTranslator(translatorId);

			foreach (var translator in resolvedTranslators) book.AddTranslator(translator);
		}

		_bookRepository.Update(book);
		return ServiceResult<Book>.Ok(book, ValidationMessages.BookUpdatedSuccessfully);
	}


	public ServiceResult<Book> RemoveBook(int bookId)
	{
		var book = FindBookById(bookId);
		if (book is null) return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetActiveLoansByBook(bookId);
		if (activeLoans.Count > 0 || !book.CanBeRemoved())
			return ServiceResult<Book>.Fail(ValidationMessages.BookRemoveFailedBorrowed);

		book.RemoveAllAuthors();
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