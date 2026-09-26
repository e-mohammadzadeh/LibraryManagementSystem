using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.DTOs.Books;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];
	private readonly List<BookAuthor> _bookAuthors = [];
	private readonly List<BookTranslator> _bookTranslators = [];


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		book.Id = Guid.CreateVersion7();
		book.CreatedAt = DateTime.UtcNow;
		book.AvailableCopies = book.TotalCopies;
		book.IsRemoved = false;
		_books.Add(book);
	}


	public Book? FindById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_books, filter).FirstOrDefault(b => b.Id == id);
	}


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter)];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.Authors.Any(a => a.AuthorId == authorId))];
	}


	public IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.Translators.Any(t => t.TranslatorId == translatorId))];
	}


	public IReadOnlyList<Book> GetAvailableBooks(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.AvailableCopies > 0)];
	}


	public bool ExistsByName(string name, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(name)) return false;

		return _books.Any(b =>
			b.Id != excludeId &&
			!b.IsRemoved &&
			b.Title.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(ISBN isbn, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(isbn)) return false;

		return _books.Any(b =>
			b.Id != excludeId &&
			!b.IsRemoved &&
			b.ISBN.Equals(isbn));
	}


	public void Remove(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (book.IsRemoved) return;
		book.IsRemoved = true;
		book.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];

		return
		[
			.. _books
				.Where(b => !b.IsRemoved)
				.Where(b =>
				{
					var value = selector(b);
					return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
				})
		];
	}


	public IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector)
	{
		return
		[
			.. _books
				.Where(b => !b.IsRemoved)
				.Where(b =>
				{
					var value = selector(b);
					return value >= from && value <= to;
				})
		];
	}


	public void Update(Book book, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(book);

		var index = _books.FindIndex(b => b.Id == book.Id);
		if (index < 0)
			throw new AuthorNotFoundException(book.Id);

		_books[index] = book;
		book.UpdatedAt = DateTime.UtcNow;
		book.UpdatedByUserId = updatedBy;
	}


	private void AddAuthor(Book book, Author author)
	{
		if (_bookAuthors.Any(ba => ba.AuthorId == author.Id)) return;

		_bookAuthors.Add(new BookAuthor(book, author));
		book.UpdatedAt = DateTime.UtcNow;
	}


	private void AddTranslator(Book book, Translator translator)
	{
		if (_bookTranslators.Any(bt => bt.TranslatorId == translator.Id)) return;

		_bookTranslators.Add(new BookTranslator(book, translator));
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void AssignAuthorsToBook(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		foreach (var author in authorList) AddAuthor(book, author);
	}


	public void AssignTranslatorsToBook(Book book, IEnumerable<Translator>? translators)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (translators is null) return;
		foreach (var translator in translators.DistinctBy(t => t.Id)) AddTranslator(book, translator);
	}


	public void RemoveAuthor(Book book, Guid authorId)
	{
		if (_bookAuthors.Count <= 1) throw new InvalidOperationException(Messages.BookRequiresAtLeastOneAuthor);

		var bookAuthor = _bookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);

		if (bookAuthor is null) return;

		_bookAuthors.Remove(bookAuthor);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void RemoveTranslator(Book book, Guid translatorId)
	{
		var bookTranslator = _bookTranslators.FirstOrDefault(bt => bt.TranslatorId == translatorId);

		if (bookTranslator is null) return;

		_bookTranslators.Remove(bookTranslator);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingIds = authorList.Select(a => a.Id).ToHashSet();
		var existingIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();

		foreach (var authorId in existingIds.Except(incomingIds)) RemoveAuthor(book, authorId);
		foreach (var author in authorList) AddAuthor(book, author);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(translators);

		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		var incomingIds = translatorList.Select(t => t.Id).ToHashSet();
		var existingIds = book.BookTranslators.Select(bt => bt.TranslatorId).ToHashSet();

		foreach (var translatorId in existingIds.Except(incomingIds)) RemoveTranslator(book, translatorId);
		foreach (var translator in translatorList) AddTranslator(book, translator);
	}


	public void DetachFromTranslators(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		foreach (var translatorId in book.BookTranslators.Select(bt => bt.TranslatorId).ToList())
			RemoveTranslator(book, translatorId);
	}


	public void BorrowCopy(Book book)
	{
		if (book.AvailableCopies <= 0) throw new InvalidOperationException("No copies are available.");
		book.AvailableCopies--;
		//TODO	(Web API)	Raise an event: a signal to the rest of the system that says "this book is now out of stock"
	}


	public void ReturnCopy(Book book)
	{
		if (book.AvailableCopies >= book.TotalCopies)
			throw new InvalidOperationException("Cannot return a copy because all copies are already in the library.");

		book.AvailableCopies++;
	}


	// ---------- Private helper ----------
	private static IEnumerable<Book> ApplyFilter(IEnumerable<Book> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(b => !b.IsRemoved),
			EntityFilter.Removed => source.Where(b => b.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}