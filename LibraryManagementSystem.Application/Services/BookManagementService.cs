using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.Mapping;
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


	public ServiceResult<BookDto> AddBook(CreateBookDto dto)
	{
		if (_bookRepository.ExistsByName(dto.BookName))
			return ServiceResult<BookDto>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (_bookRepository.ExistsByISBN(dto.ISBN))
			return ServiceResult<BookDto>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<BookDto>.Fail(ValidationMessages.InvalidGenre);
		var genre = (Genre)dto.GenreId;

		if (dto.AuthorIds.Count is 0)
			return ServiceResult<BookDto>.Fail(ValidationMessages.BookRequiresAtLeastOneAuthor);

		if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(ValidationMessages.DuplicateAuthorsNotAllowed);

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds)
		{
			var author = _authorRepository.FindById(authorId);
			if (author is null)
				return ServiceResult<BookDto>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, authorId));
			authors.Add(author);
		}

		if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(ValidationMessages.DuplicateTranslatorsNotAllowed);

		var translators = new List<Translator>();
		foreach (var translatorId in dto.TranslatorIds)
		{
			var translator = _translatorRepository.FindById(translatorId);
			if (translator is null) return ServiceResult<BookDto>.Fail(ValidationMessages.NotTranslatorMatched);
			translators.Add(translator);
		}

		var newBook = new Book(dto.ISBN, dto.BookName, authors, translators, dto.PublishDate, dto.TotalCopies, genre,
			dto.Publisher, dto.Description);

		_bookRepository.Add(newBook);
		return ServiceResult<BookDto>.Ok(newBook.ToDto(), ValidationMessages.BookAddedSuccessfully);
	}


	public IReadOnlyList<BookDto> GetAllBooks()
	{
		return _bookRepository.GetAll().Select(book => book.ToDto()).ToList().AsReadOnly();
	}


	public BookDto? FindBookById(int id)
	{
		var book = _bookRepository.FindById(id);
		return book?.ToDto();
	}


	public ServiceResult<BookDto> UpdateBook(int bookId, UpdateBookDto dto)
	{
		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<BookDto>.Fail(ValidationMessages.NotAvailableBook);

		if (dto.BookName != null && _bookRepository.ExistsByName(dto.BookName, bookId))
			return ServiceResult<BookDto>.Fail(ValidationMessages.FailureDuplicateBookByName);

		if (dto.ISBN != null && _bookRepository.ExistsByISBN(dto.ISBN, bookId))
			return ServiceResult<BookDto>.Fail(ValidationMessages.FailureDuplicateBookByISBN);

		if (dto.GenreId != null && !Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<BookDto>.Fail(ValidationMessages.InvalidGenre);

		if (dto.TotalCopies is <= 0)
			return ServiceResult<BookDto>.Fail(ValidationMessages.WrongTotalCopies);

		List<Author>? resolvedAuthors = null;
		if (dto.AuthorIds is not null)
		{
			if (dto.AuthorIds.Count == 0)
				return ServiceResult<BookDto>.Fail(ValidationMessages.BookRequiresAtLeastOneAuthor);

			if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(ValidationMessages.DuplicateAuthorsNotAllowed);


			resolvedAuthors = new List<Author>();
			foreach (var id in dto.AuthorIds.Distinct())
			{
				var author = _authorRepository.FindById(id);
				if (author is null)
					return ServiceResult<BookDto>.Fail(string.Format(ValidationMessages.AuthorNotFoundFormat, id));
				resolvedAuthors.Add(author);
			}
		}

		List<Translator>? resolvedTranslators = null;
		if (dto.TranslatorIds is not null)
		{
			if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(ValidationMessages.DuplicateTranslatorsNotAllowed);

			resolvedTranslators = new List<Translator>();
			foreach (var translatorId in dto.TranslatorIds)
			{
				var translator = _translatorRepository.FindById(translatorId);
				if (translator is null)
					return ServiceResult<BookDto>.Fail(string.Format(ValidationMessages.TranslatorNotFoundFormat,
						translatorId));
				resolvedTranslators.Add(translator);
			}
		}

		Genre? genre = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genre, dto.Publisher, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<BookDto>.Fail(ValidationMessages.TotalCopiesUpdateInvalid);

		if (resolvedAuthors is not null) book.ReplaceAuthors(resolvedAuthors);

		if (resolvedTranslators is not null) book.ReplaceTranslators(resolvedTranslators);

		_bookRepository.Update(book);
		return ServiceResult<BookDto>.Ok(book.ToDto(), ValidationMessages.BookUpdatedSuccessfully);
	}


	public ServiceResult<BookDto> RemoveBook(int bookId)
	{
		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<BookDto>.Fail(ValidationMessages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetActiveLoansByBook(bookId);
		if (activeLoans.Count > 0 || !book.CanBeRemoved())
		{
			var borrowersId = string.Join(", ", activeLoans.Select(al => al.UserId).ToList());
			return ServiceResult<BookDto>.Fail(string.Format(ValidationMessages.BookRemoveFailedBorrowed, borrowersId));
		}

		book.DetachFromAuthors();
		book.DetachFromTranslators();
		_bookRepository.Remove(book);
		return ServiceResult<BookDto>.Ok(book.ToDto(), ValidationMessages.BookRemovedSuccessfully);
	}


	public IReadOnlyList<BookDto> SearchBooks<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		return _bookRepository.Search(searchTerm, selector, comparer).Select(book => book.ToDto()).ToList()
			.AsReadOnly();
	}


	public IReadOnlyList<BookDto> SearchBooks<T>(T? searchTerm, Func<Book, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		return _bookRepository.Search(searchTerm, selector, comparer).Select(book => book.ToDto()).ToList()
			.AsReadOnly();
	}


	public IReadOnlyList<BookDto> GetAvailableBooks()
	{
		return _bookRepository.GetAvailableBooks().Select(book => book.ToDto()).ToList().AsReadOnly();
	}
}