using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;
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
		_books.Add(book);
		book.UpdatedAt = DateTime.UtcNow;
	}


	public Book? FindById(Guid id) { return _books.FirstOrDefault(book => book.Id == id); }


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		var query = _books.AsEnumerable();
		switch (filter)
		{
			case EntityFilter.Active:
				query = query.Where(b => !b.IsRemoved);
				break;
			case EntityFilter.Removed:
				query = query.Where(b => b.IsRemoved);
				break;
			case EntityFilter.All:
				// No filter – include everyone
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
		}

		return [.. query];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid authorId)
	{
		return [.. _books.Where(book => book.BookAuthors.Any(ba => ba.AuthorId == authorId))];
	}


	public IReadOnlyList<Book> GetByTranslatorId(Guid translatorId)
	{
		return [.. _books.Where(book => book.BookTranslators.Any(bt => bt.TranslatorId == translatorId))];
	}


	public bool ExistsByName(string name, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(name)) return false;

		return _books.Any(book => book.Id != excludeId && book.Title.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByISBN(string isbn, Guid? excludeId = null)
	{
		if (string.IsNullOrWhiteSpace(isbn)) return false;

		return _books.Any(book =>
			book.Id != excludeId &&
			book.InternationalStandardBookNumber.Equals(isbn, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Book> GetAvailableBooks()
	{
		return [.. _books.Where(b => b is { AvailableCopies: > 0, IsRemoved: false })];
	}


	public void Remove(Book book)
	{
		if (book.IsRemoved) return;
		book.IsRemoved = true;
		book.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Book> Search(string searchTerm, Func<Book, string?> selector)
	{
		return
		[
			.. _books.Where(book =>
			{
				var value = selector(book);
				return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public IReadOnlyList<Book> SearchByDate(DateOnly from, DateOnly to, Func<Book, DateOnly> selector)
	{
		return
		[
			.. _books.Where(book =>
			{
				var value = selector(book);
				return value >= from && value <= to;
			})
		];
	}


	public void Update(Book book, UpdateBookDto dto)
	{
		book.Title = dto.BookName ?? book.Title;
		book.InternationalStandardBookNumber = dto.ISBN ?? book.InternationalStandardBookNumber;
		book.PublishDate = dto.PublishDate ?? book.PublishDate;
		book.Genre = dto.Genre ?? book.Genre;
		book.Publisher = dto.Publisher ?? book.Publisher;
		book.TotalCopies = dto.TotalCopies ?? book.TotalCopies;
		book.Description = dto.Description ?? book.Description;
		book.UpdatedAt = DateTime.UtcNow;
	}


	private void AddAuthor(Book book, Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		if (_bookAuthors.Any(ba => ba.AuthorId == author.Id)) return;

		_bookAuthors.Add(new BookAuthor(book, author));
		book.UpdatedAt = DateTime.UtcNow;
	}


	private void AddTranslator(Book book, Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

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



	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingIds = authorList.Select(a => a.Id).ToHashSet();
		var existingIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();

		foreach (var authorId in existingIds.Except(incomingIds)) book.RemoveAuthor(authorId);
		foreach (var author in authorList) AddAuthor(book, author);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(translators);

		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		var incomingIds = translatorList.Select(t => t.Id).ToHashSet();
		var existingIds = book.BookTranslators.Select(bt => bt.TranslatorId).ToHashSet();

		foreach (var translatorId in existingIds.Except(incomingIds)) book.RemoveTranslator(translatorId);
		foreach (var translator in translatorList) AddTranslator(book, translator);
	}


	public void DetachFromTranslators(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		foreach (var translatorId in book.BookTranslators.Select(bt => bt.TranslatorId).ToList())
			book.RemoveTranslator(translatorId);
	}
}