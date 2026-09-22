using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Rules;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;

namespace LibraryManagementSystem.Application.Services;

public class BookManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IBookRepository _bookRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IAuditLogManagementService _auditLog;
	private readonly IAuthorizationService _authorization;


	public BookManagementService(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository, ILoanRepository loanRepository, IAuditLogManagementService auditLog,
		IAuthorizationService authorizationService)
	{
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_bookRepository = bookRepository;
		_loanRepository = loanRepository;
		_auditLog = auditLog;
		_authorization = authorizationService;
	}


	public ServiceResult<BookDto> AddBook(CreateBookDto dto)
	{
		if (_bookRepository.ExistsByName(dto.BookName, null))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		if (_bookRepository.ExistsByISBN(dto.ISBN, null))
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

		if (authors.Count == 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

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

		var newBook = new Book(dto.ISBN, dto.BookName, dto.PublishDate, dto.TotalCopies, genre, dto.Publisher,
			dto.Description);

		_bookRepository.AssignAuthorsToBook(newBook, authors);
		_bookRepository.AssignTranslatorsToBook(newBook, translators);
		_bookRepository.Add(newBook);
		_auditLog.Record(AuditAction.BookCreated, "Book", newBook.Id, "Book created.");

		return ServiceResult<BookDto>.Ok(newBook.ToDto(), Messages.BookAddedSuccessfully);
	}


	public IReadOnlyList<BookDto> GetAllBooks(BookSortField sortField = BookSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var books = _bookRepository.GetAll(EntityFilter.Active).ToList();

		var sortedBooks = sortField switch
		{
			BookSortField.Id => ApplySort(books, book => book.Id, sortDirection),
			BookSortField.Name => ApplySort(books, book => book.Title, sortDirection),
			BookSortField.ISBN => ApplySort(books, book => book.InternationalStandardBookNumber, sortDirection),
			BookSortField.PublishDate => ApplySort(books, book => book.PublishDate, sortDirection),
			BookSortField.Genre => ApplySort(books, book => book.Genre, sortDirection),
			BookSortField.Publisher => ApplySort(books, book => book.Publisher, sortDirection),
			BookSortField.AvailableCopies => ApplySort(books, book => book.AvailableCopies, sortDirection),
			BookSortField.Author => ApplySort(books,
				book => string.Join(", ",
					book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").Order()),
				sortDirection),

			BookSortField.Translator => ApplySort(books,
				book => string.Join(", ",
					book.BookTranslators.Select(bt => $"{bt.Translator.FirstName} {bt.Translator.LastName}").Order()),
				sortDirection),

			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		return sortedBooks.Select(book => book.ToDto()).ToList();
	}


	private static IOrderedEnumerable<Book> ApplySort<TKey>(IEnumerable<Book> books, Func<Book, TKey> keySelector,
		SortDirection sortDirection)
	{
		return sortDirection == SortDirection.Ascending
			? books.OrderBy(keySelector)
			: books.OrderByDescending(keySelector);
	}


	public BookDto? FindBookById(Guid id)
	{
		var book = _bookRepository.FindById(id);
		return book?.ToDto();
	}


	public ServiceResult<BookDto> UpdateBook(Guid bookId, UpdateBookDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var book = _bookRepository.FindById(bookId);
		if (book is null || book.IsRemoved) return ServiceResult<BookDto>.Fail(Messages.NotAvailableBook);

		if (IsNoOpUpdateBook(book, dto)) return ServiceResult<BookDto>.Fail(Messages.NoChangesDetected);

		if (dto.BookName != null && _bookRepository.ExistsByName(dto.BookName, bookId))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		if (dto.ISBN != null && _bookRepository.ExistsByISBN(dto.ISBN, bookId))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByISBN);

		if (dto.Genre != null && !Enum.IsDefined(dto.Genre.Value))
			return ServiceResult<BookDto>.Fail(Messages.InvalidGenre);

		if (dto.TotalCopies is <= 0) return ServiceResult<BookDto>.Fail(Messages.WrongTotalCopies);

		List<Author>? resolvedAuthors = null;
		if (dto.AuthorIds is not null)
		{
			if (dto.AuthorIds.Count == 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

			if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(Messages.DuplicateAuthorsNotAllowed);

			resolvedAuthors = [];
			foreach (var id in dto.AuthorIds)
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

		var auditDetails = BookUpdateAuditDetailsBuilder.BuildBookUpdateAuditDetails(book, dto, resolvedAuthors, resolvedTranslators);

		if (dto.TotalCopies.HasValue)
		{
			var newTotalCopies = dto.TotalCopies.Value;
			if (!BookInventoryRules.CanChangeTotalCopies(book.TotalCopies, book.AvailableCopies, newTotalCopies))
				return ServiceResult<BookDto>.Fail(Messages.TotalCopiesUpdateInvalid);
			var difference = newTotalCopies - book.TotalCopies;
			book.TotalCopies = newTotalCopies;
			book.AvailableCopies += difference;
		}

		if (resolvedAuthors is not null) _bookRepository.ReplaceAuthors(book, resolvedAuthors);
		if (resolvedTranslators is not null) _bookRepository.ReplaceTranslators(book, resolvedTranslators);

		_bookRepository.Update(book, dto);
		_auditLog.Record(AuditAction.BookUpdated, "Book", bookId, auditDetails ?? "Book updated.");
		return ServiceResult<BookDto>.Ok(book.ToDto(), Messages.BookUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateBook(Book book, UpdateBookDto dto)
	{
		return (dto.BookName == null || dto.BookName == book.Title) &&
		       (dto.ISBN == null || dto.ISBN == book.InternationalStandardBookNumber) &&
		       (dto.AuthorIds == null || SameIds(dto.AuthorIds, book.BookAuthors.Select(ba => ba.AuthorId))) &&
		       (dto.TranslatorIds == null ||
		        SameIds(dto.TranslatorIds, book.BookTranslators.Select(bt => bt.TranslatorId))) &&
		       (dto.PublishDate == null || dto.PublishDate == book.PublishDate) &&
		       (dto.Genre == null || dto.Genre == book.Genre) &&
		       (dto.Publisher == null || dto.Publisher == book.Publisher) &&
		       (dto.TotalCopies == null || dto.TotalCopies == book.TotalCopies) &&
		       (dto.Description == null || dto.Description == book.Description);
	}


	private static bool SameIds(IEnumerable<Guid> left, IEnumerable<Guid> right)
	{
		var a = left.Distinct().OrderBy(x => x).ToList();
		var b = right.Distinct().OrderBy(x => x).ToList();
		return a.SequenceEqual(b);
	}


	public ServiceResult<BookDto> RemoveBook(Guid bookId)
	{
		var book = _bookRepository.FindById(bookId);
		if (book is null || book.IsRemoved) return ServiceResult<BookDto>.Fail(Messages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetActiveLoansByBook(bookId);
		if (activeLoans.Count > 0)
		{
			var borrowersId = string.Join(", ", activeLoans.Select(al => al.UserId));
			return ServiceResult<BookDto>.Fail(string.Format(Messages.BookRemoveFailedBorrowed, borrowersId));
		}

		if (book.TotalCopies != book.AvailableCopies) return ServiceResult<BookDto>.Fail(Messages.BookRemoveFailed);

		_bookRepository.Remove(book);
		_auditLog.Record(AuditAction.BookRemoved, "Book", bookId, "Book removed.");

		return ServiceResult<BookDto>.Ok(book.ToDto(), Messages.BookRemovedSuccessfully);
	}


	public IReadOnlyList<BookDto> SearchBooks(string searchTerm, BookSearchField field)
	{
		Func<Book, string?> selector = field switch
		{
			BookSearchField.BookName => b => b.Title,
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


	public IReadOnlyList<BookDto> SearchBooksByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector)
	{
		return [.. _bookRepository.SearchByDate(from, to, selector).Select(book => book.ToDto())];
	}


	public IReadOnlyList<BookDto> GetAvailableBooks()
	{
		return [.. _bookRepository.GetAvailableBooks().Select(book => book.ToDto())];
	}


	public IReadOnlyList<BookDto> GetRemovedBooks()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedAuthors)) return [];
		return [.. _bookRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}
}