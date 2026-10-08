using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Rules;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.ValueObjects;

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
		if (_bookRepository.ExistsByName(dto.Title, null))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		var isbn = ISBN.Create(dto.ISBN);
		if (_bookRepository.ExistsByISBN(isbn, null))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByISBN);

		if (!Enum.IsDefined(dto.Genre)) return ServiceResult<BookDto>.Fail(Messages.InvalidGenre);
		var genre = BookGenre.Create(dto.Genre);

		if (!Enum.IsDefined(dto.OriginalLanguage) ||
		    (dto.TranslatedLanguage != null && !Enum.IsDefined(dto.TranslatedLanguage.Value)))
			return ServiceResult<BookDto>.Fail(Messages.InvalidLanguage);

		if (dto.AuthorIds.Count is 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

		if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(Messages.DuplicateAuthorsNotAllowed);

		var authors = new List<Author>();
		foreach (var authorId in dto.AuthorIds)
		{
			var author = _authorRepository.FindById(authorId, EntityFilter.Active);
			if (author is null)
				return ServiceResult<BookDto>.Fail(string.Format(Messages.AuthorNotFoundFormat, authorId));
			authors.Add(author);
		}

		if (authors.Count == 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

		if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
			return ServiceResult<BookDto>.Fail(Messages.DuplicateTranslatorsNotAllowed);

		var translators = new List<Translator>();
		foreach (var translator in
		         dto.TranslatorIds.Select(translatorId =>
			         _translatorRepository.FindById(translatorId, EntityFilter.Active)))
		{
			if (translator is null) return ServiceResult<BookDto>.Fail(Messages.NotTranslatorMatched);
			translators.Add(translator);
		}

		if (dto.TotalCopies <= 0) return ServiceResult<BookDto>.Fail(Messages.WrongTotalCopies);

		var newBook = new Book
		{
			Title = dto.Title,
			ISBN = isbn,
			PublishDate = dto.PublishDate,
			Genre = genre,
			Publisher = dto.Publisher,
			OriginalLanguage = dto.OriginalLanguage,
			TotalCopies = dto.TotalCopies,
			Description = dto.Description
		};

		_bookRepository.AssignAuthorsToBook(newBook, authors);
		_bookRepository.AssignTranslatorsToBook(newBook, translators, dto.TranslatedLanguage);
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
			BookSortField.ISBN => ApplySort(books, book => book.ISBN, sortDirection),
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
		var book = _bookRepository.FindById(id, EntityFilter.Active);
		return book?.ToDto();
	}


	public ServiceResult<BookDto> UpdateBook(UpdateBookDto dto, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var book = _bookRepository.FindById(dto.Id, EntityFilter.Active);
		if (book is null || book.IsRemoved) return ServiceResult<BookDto>.Fail(Messages.NotAvailableBook);

		if (IsNoOpUpdateBook(book, dto)) return ServiceResult<BookDto>.Fail(Messages.NoChangesDetected);

		if (dto.BookName != null && _bookRepository.ExistsByName(dto.BookName, dto.Id))
			return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByName);

		if (dto.ISBN != null)
		{
			var isbn = ISBN.Create(dto.ISBN);
			if (_bookRepository.ExistsByISBN(isbn, dto.Id))
				return ServiceResult<BookDto>.Fail(Messages.DuplicateBooksNotAllowedByISBN);
		}

		if (dto.Genre != null && !Enum.IsDefined(dto.Genre.Value))
			return ServiceResult<BookDto>.Fail(Messages.InvalidGenre);

		if (dto.TotalCopies is <= 0) return ServiceResult<BookDto>.Fail(Messages.WrongTotalCopies);

		List<Author>? resolvedAuthors = null;
		if (dto.AuthorIds != null)
		{
			if (dto.AuthorIds.Count == 0) return ServiceResult<BookDto>.Fail(Messages.BookRequiresAtLeastOneAuthor);

			if (dto.AuthorIds.Count != dto.AuthorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(Messages.DuplicateAuthorsNotAllowed);

			resolvedAuthors = [];
			foreach (var id in dto.AuthorIds)
			{
				var author = _authorRepository.FindById(id, EntityFilter.Active);
				if (author is null)
					return ServiceResult<BookDto>.Fail(string.Format(Messages.AuthorNotFoundFormat, id));
				resolvedAuthors.Add(author);
			}
		}

		List<Translator>? resolvedTranslators = null;
		if (dto.TranslatorIds != null)
		{
			if (dto.TranslatorIds.Count != dto.TranslatorIds.Distinct().Count())
				return ServiceResult<BookDto>.Fail(Messages.DuplicateTranslatorsNotAllowed);

			resolvedTranslators = [];
			foreach (var translatorId in dto.TranslatorIds)
			{
				var translator = _translatorRepository.FindById(translatorId, EntityFilter.Active);
				if (translator is null)
					return ServiceResult<BookDto>.Fail(string.Format(Messages.TranslatorNotFoundFormat,
						translatorId));
				resolvedTranslators.Add(translator);
			}
		}

		var auditDetails =
			BookUpdateAuditDetailsBuilder.BuildBookUpdateAuditDetails(book, dto, resolvedAuthors, resolvedTranslators);

		if (dto.TotalCopies.HasValue)
		{
			var newTotalCopies = dto.TotalCopies.Value;
			if (!BookInventoryRules.CanChangeTotalCopies(book.TotalCopies, book.AvailableCopies, newTotalCopies))
				return ServiceResult<BookDto>.Fail(Messages.TotalCopiesUpdateInvalid);
			var difference = newTotalCopies - book.TotalCopies;
			book.TotalCopies = newTotalCopies;
			book.AvailableCopies += difference;
		}

		if (resolvedAuthors != null) _bookRepository.ReplaceAuthors(book, resolvedAuthors);
		if (resolvedTranslators != null)
			_bookRepository.ReplaceTranslators(book, resolvedTranslators, dto.TranslatedLanguage);

		_bookRepository.Update(book, updatedBy);
		_auditLog.Record(AuditAction.BookUpdated, "Book", dto.Id, auditDetails ?? "Book updated.");
		return ServiceResult<BookDto>.Ok(book.ToDto(), Messages.BookUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateBook(Book book, UpdateBookDto dto)
	{
		return (dto.BookName == null || dto.BookName == book.Title) &&
		       (dto.ISBN == null || dto.ISBN == book.ISBN) &&
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
		var book = _bookRepository.FindById(bookId, EntityFilter.Active);
		if (book is null || book.IsRemoved) return ServiceResult<BookDto>.Fail(Messages.BookRemoveFailed);

		var activeLoans = _loanRepository.GetLoansByBook(bookId, LoanFilter.Active);
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
			BookSearchField.ISBN => b => b.ISBN,
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


	public IReadOnlyList<BookDto> GetAvailableBooks(EntityFilter filter)
	{
		return [.. _bookRepository.GetAvailableBooks(filter).Select(book => book.ToDto())];
	}


	public IReadOnlyList<BookDto> GetRemovedBooks()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedAuthors)) return [];
		return [.. _bookRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}







	public void AssignAuthorsToBook(Book book, IEnumerable<Author> authors) {
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		foreach (var author in authorList)
			AddAuthor(book, author);
	}


	public void AssignTranslatorsToBook(Book book, IEnumerable<Translator>? translators, Language? language) {
		ArgumentNullException.ThrowIfNull(book);

		if (translators is null)
			return;
		foreach (var translator in translators.DistinctBy(t => t.Id))
			AddTranslator(book, translator, language);
	}


	public void RemoveAuthor(Book book, Guid authorId) {
		if (book.BookAuthors.Count <= 1)
			throw new InvalidOperationException(Messages.BookRequiresAtLeastOneAuthor);

		var bookAuthor = book.BookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);

		if (bookAuthor is null)
			return;

		book.BookAuthors.Remove(bookAuthor);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void RemoveTranslator(Book book, Guid translatorId) {
		var bookTranslator = book.BookTranslators.FirstOrDefault(bt => bt.TranslatorId == translatorId);

		if (bookTranslator is null)
			return;

		book.BookTranslators.Remove(bookTranslator);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors) {
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0)
			throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingIds = authorList.Select(a => a.Id).ToHashSet();
		var existingIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();

		foreach (var authorId in existingIds.Except(incomingIds))
			RemoveAuthor(book, authorId);
		foreach (var author in authorList)
			AddAuthor(book, author);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators, Language language) {
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(translators);

		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		var incomingIds = translatorList.Select(t => t.Id).ToHashSet();
		var existingIds = book.BookTranslators.Select(bt => bt.TranslatorId).ToHashSet();

		foreach (var translatorId in existingIds.Except(incomingIds))
			RemoveTranslator(book, translatorId);
		foreach (var translator in translatorList)
			AddTranslator(book, translator, language);
	}


	public void DetachFromTranslators(Book book) {
		ArgumentNullException.ThrowIfNull(book);

		foreach (var translatorId in book.BookTranslators.Select(bt => bt.TranslatorId).ToList())
			RemoveTranslator(book, translatorId);
	}


	public void BorrowCopy(Book book) {
		if (book.AvailableCopies <= 0)
			throw new InvalidOperationException("No copies are available.");
		book.AvailableCopies--;
		//TODO	(Web API)	Raise an event: a signal to the rest of the system that says "this book is now out of stock"
	}


	public void ReturnCopy(Book book) {
		if (book.AvailableCopies >= book.TotalCopies)
			throw new InvalidOperationException(
				"Cannot return a copy because all copies are already in the library.");

		book.AvailableCopies++;
	}


	
	private static void AddAuthor(Book book, Author author) {
		if (book.BookAuthors.Any(ba => ba.AuthorId == author.Id))
			return;

		book.BookAuthors.Add(new BookAuthor
		{
			BookId = book.Id,
			Book = book,
			AuthorId = author.Id,
			Author = author
		});
		book.UpdatedAt = DateTime.UtcNow;
	}


	private static void AddTranslator(Book book, Translator translator, Language? language) {
		if (book.BookTranslators.Any(bt => bt.TranslatorId == translator.Id))
			return;

		book.BookTranslators.Add(new BookTranslator
		{
			Book = book,
			BookId = book.Id,
			TranslatorId = translator.Id,
			Translator = translator,
			TranslationLanguage = language ?? Language.Arabic   // Should fix here: no translator = no language
		});
		book.UpdatedAt = DateTime.UtcNow;
	}
}