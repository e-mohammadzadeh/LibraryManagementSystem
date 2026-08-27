using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Search;
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
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		if (_bookRepository.ExistsByISBN(dto.ISBN))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByISBN);

		if (!Enum.IsDefined(typeof(Genre), dto.GenreId)) return ServiceResult<BookDto>.Fail(Messages.InvalidGenre);
		var genre = (Genre)dto.GenreId;

		if (dto.AuthorIds.Count is 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

		if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(Messages.DuplicateAuthorsNotAllowed);

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds)
		{
			var author = _authorRepository.FindById(authorId);
			if (author is null)
				return ServiceResult<BookDto>.Fail(string.Format(Messages.AuthorNotFoundFormat, authorId));
			authors.Add(author);
		}

		if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(Messages.DuplicateTranslatorsNotAllowed);

		var translators = new List<Translator>();
		foreach (var translator in
		         dto.TranslatorIds.Select(translatorId => _translatorRepository.FindById(translatorId)))
		{
			if (translator is null) return ServiceResult<BookDto>.Fail(Messages.NotTranslatorMatched);
			translators.Add(translator);
		}

		if (dto.TotalCopies <= 0) return ServiceResult<BookDto>.Fail(Messages.WrongTotalCopies);


		var newBook = new Book(dto.ISBN, dto.BookName, authors, translators, dto.PublishDate, dto.TotalCopies, genre,
			dto.Publisher, dto.Description);

		_bookRepository.Add(newBook);
		return ServiceResult<BookDto>.Ok(newBook.ToDto(), Messages.BookAddedSuccessfully);
	}


	public IReadOnlyList<BookDto> GetAllBooks() { return [.. _bookRepository.GetAll().Select(book => book.ToDto())]; }


	public BookDto? FindBookById(int id)
	{
		var book = _bookRepository.FindById(id);
		return book?.ToDto();
	}


	public ServiceResult<BookDto> UpdateBook(int bookId, UpdateBookDto dto)
	{
		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<BookDto>.Fail(Messages.NotAvailableBook);

		if (IsNoOpUpdateBook(book, dto)) return ServiceResult<BookDto>.Fail(Messages.NoChangesDetected);

		if (dto.BookName != null && _bookRepository.ExistsByName(dto.BookName, bookId))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		if (dto.ISBN != null && _bookRepository.ExistsByISBN(dto.ISBN, bookId))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByISBN);

		if (dto.GenreId != null && !Enum.IsDefined(typeof(Genre), dto.GenreId))
			return ServiceResult<BookDto>.Fail(Messages.InvalidGenre);

		if (dto.TotalCopies is <= 0) return ServiceResult<BookDto>.Fail(Messages.WrongTotalCopies);

		List<Author>? resolvedAuthors = null;
		if (dto.AuthorIds is not null)
		{
			if (dto.AuthorIds.Count == 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

			if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(Messages.DuplicateAuthorsNotAllowed);


			resolvedAuthors = [];
			foreach (var id in dto.AuthorIds.Distinct())
			{
				var author = _authorRepository.FindById(id);
				if (author is null)
					return ServiceResult<BookDto>.Fail(string.Format(Messages.AuthorNotFoundFormat, id));
				resolvedAuthors.Add(author);
			}
		}

		List<Translator>? resolvedTranslators = null;
		if (dto.TranslatorIds is not null)
		{
			if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(Messages.DuplicateTranslatorsNotAllowed);

			resolvedTranslators = [];
			foreach (var translatorId in dto.TranslatorIds)
			{
				var translator = _translatorRepository.FindById(translatorId);
				if (translator is null)
					return ServiceResult<BookDto>.Fail(string.Format(Messages.TranslatorNotFoundFormat,
						translatorId));
				resolvedTranslators.Add(translator);
			}
		}

		Genre? genre = dto.GenreId.HasValue ? (Genre)dto.GenreId.Value : null;

		if (!book.Update(dto.BookName, dto.ISBN, dto.PublishDate, genre, dto.Publisher, dto.TotalCopies,
			    dto.Description))
			return ServiceResult<BookDto>.Fail(Messages.TotalCopiesUpdateInvalid);

		if (resolvedAuthors is not null) book.ReplaceAuthors(resolvedAuthors);

		if (resolvedTranslators is not null) book.ReplaceTranslators(resolvedTranslators);

		_bookRepository.Update(book);
		return ServiceResult<BookDto>.Ok(book.ToDto(), Messages.BookUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateBook(Book book, UpdateBookDto dto)
	{
		return (dto.BookName == null || dto.BookName == book.BookName) &&
		       (dto.ISBN == null || dto.ISBN == book.InternationalStandardBookNumber) &&
		       (dto.AuthorIds == null || SameIds(dto.AuthorIds, book.BookAuthors.Select(ba => ba.AuthorId))) &&
		       (dto.TranslatorIds == null ||
		        SameIds(dto.TranslatorIds, book.BookTranslators.Select(bt => bt.TranslatorId))) &&
		       (dto.PublishDate == null || dto.PublishDate == book.PublishDate) &&
		       (dto.GenreId == null || dto.GenreId == (int)book.Genre) &&
		       (dto.Publisher == null || dto.Publisher == book.Publisher) &&
		       (dto.TotalCopies == null || dto.TotalCopies == book.TotalCopies) &&
		       (dto.Description == null || dto.Description == book.Description);
	}


	private static bool SameIds(IEnumerable<int> left, IEnumerable<int> right)
	{
		var a = left.Distinct().OrderBy(x => x).ToList();
		var b = right.Distinct().OrderBy(x => x).ToList();
		return a.SequenceEqual(b);
	}


	public ServiceResult<BookDto> RemoveBook(int bookId)
	{
		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<BookDto>.Fail(Messages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetActiveLoansByBook(bookId);
		if (activeLoans.Count > 0)
		{
			var borrowersId = string.Join(", ", activeLoans.Select(al => al.UserId));
			return ServiceResult<BookDto>.Fail(string.Format(Messages.BookRemoveFailedBorrowed, borrowersId));
		}

		if (!book.CanBeRemoved()) return ServiceResult<BookDto>.Fail(Messages.BookRemoveFailed);

		book.DetachFromAuthors();
		book.DetachFromTranslators();
		_bookRepository.Remove(book);
		return ServiceResult<BookDto>.Ok(book.ToDto(), Messages.BookRemovedSuccessfully);
	}


	public IReadOnlyList<BookDto> SearchBooks(string searchTerm, BookSearchField field)
	{
		Func<Book, string?> selector = field switch
		{
			BookSearchField.BookName => b => b.BookName,
			BookSearchField.ISBN => b => b.InternationalStandardBookNumber,
			BookSearchField.AuthorName => b =>
				string.Join(", ", b.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}")),
			BookSearchField.TranslatorName => b =>
				b.BookTranslators.Count == 0
					? null
					: string.Join(", ",
						b.BookTranslators.Select(bt => $"{bt.Translator.FirstName} {bt.Translator.LastName}")),
			BookSearchField.PublishDate => b => b.PublishDate.ToString("yyyy-MM-dd"),
			BookSearchField.Genre => b => b.Genre.ToString(),
			BookSearchField.Publisher => b => b.Publisher,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _bookRepository.Search(searchTerm, selector).Select(book => book.ToDto())];
	}



	public IReadOnlyList<BookDto> GetAvailableBooks()
	{
		return [.. _bookRepository.GetAvailableBooks().Select(book => book.ToDto())];
	}
}