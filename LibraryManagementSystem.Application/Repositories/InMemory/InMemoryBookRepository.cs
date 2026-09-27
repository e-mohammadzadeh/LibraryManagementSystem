using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books = [];



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

		var tracked = _books.FirstOrDefault(b => b.Id == book.Id) ?? throw new BookNotFoundException(book.Id);

		tracked.Title = book.Title;
		tracked.ISBN = book.ISBN;
		tracked.Authors = book.Authors;
		tracked.Translators = book.Translators;
		tracked.PublishDate = book.PublishDate;
		tracked.Genre = book.Genre;
		tracked.Publisher = book.Publisher;
		tracked.OriginalLanguage = book.OriginalLanguage;
		tracked.TotalCopies = book.TotalCopies;
		tracked.AvailableCopies = book.AvailableCopies;
		tracked.Description = book.Description;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = updatedBy;
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
		if (book.Authors.Count <= 1) throw new InvalidOperationException(Messages.BookRequiresAtLeastOneAuthor);

		var bookAuthor = book.Authors.FirstOrDefault(ba => ba.AuthorId == authorId);

		if (bookAuthor is null) return;

		book.Authors.Remove(bookAuthor);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void RemoveTranslator(Book book, Guid translatorId)
	{
		var bookTranslator = book.Translators.FirstOrDefault(bt => bt.TranslatorId == translatorId);

		if (bookTranslator is null) return;

		book.Translators.Remove(bookTranslator);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingIds = authorList.Select(a => a.Id).ToHashSet();
		var existingIds = book.Authors.Select(ba => ba.AuthorId).ToHashSet();

		foreach (var authorId in existingIds.Except(incomingIds)) RemoveAuthor(book, authorId);
		foreach (var author in authorList) AddAuthor(book, author);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(translators);

		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		var incomingIds = translatorList.Select(t => t.Id).ToHashSet();
		var existingIds = book.Translators.Select(bt => bt.TranslatorId).ToHashSet();

		foreach (var translatorId in existingIds.Except(incomingIds)) RemoveTranslator(book, translatorId);
		foreach (var translator in translatorList) AddTranslator(book, translator);
	}


	public void DetachFromTranslators(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		foreach (var translatorId in book.Translators.Select(bt => bt.TranslatorId).ToList())
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
			throw new InvalidOperationException(
				"Cannot return a copy because all copies are already in the library.");

		book.AvailableCopies++;
	}


	private static void AddAuthor(Book book, Author author)
	{
		if (book.Authors.Any(ba => ba.AuthorId == author.Id)) return;

		book.Authors.Add(new BookAuthor
		{
			BookId = book.Id,
			Book = book,
			AuthorId = author.Id,
			Author = author
		});
		book.UpdatedAt = DateTime.UtcNow;
	}


	private static void AddTranslator(Book book, Translator translator)
	{
		if (book.Translators.Any(bt => bt.TranslatorId == translator.Id)) return;

		book.Translators.Add(new BookTranslator
		{
			Book = book,
			BookId = book.Id,
			TranslatorId = translator.Id,
			Translator = translator
			//TranslationLanguage = 
		});
		book.UpdatedAt = DateTime.UtcNow;
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