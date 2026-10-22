using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookAuthorRepository : IBookAuthorRepository
{
	private readonly List<BookAuthor> _bookAuthors = [];


	public void Add(BookAuthor bookAuthor)
	{
		ArgumentNullException.ThrowIfNull(bookAuthor);

		var alreadyExists = _bookAuthors.Any(ba =>
			ba.BookId == bookAuthor.BookId &&
			ba.AuthorId == bookAuthor.AuthorId);

		if (alreadyExists) throw new InvalidOperationException("This author is already associated with this book.");

		_bookAuthors.Add(bookAuthor);
	}


	public void Remove(BookAuthor bookAuthor) { ArgumentNullException.ThrowIfNull(bookAuthor); }


	public void Update(BookAuthor bookAuthor)
	{
		ArgumentNullException.ThrowIfNull(bookAuthor);

		var tracked = _bookAuthors.FirstOrDefault(ba =>
			              ba.BookId == bookAuthor.BookId &&
			              ba.AuthorId == bookAuthor.AuthorId) ??
		              throw new BookAuthorNotFoundException(bookAuthor.AuthorId, bookAuthor.BookId);

		tracked.Author = bookAuthor.Author;
		tracked.AuthorId = bookAuthor.AuthorId;
		tracked.Book = bookAuthor.Book;
		tracked.BookId = bookAuthor.BookId;
	}


	public BookAuthor? GetByBookId(Guid bookId, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_bookAuthors, filter).FirstOrDefault(ba => ba.BookId == bookId);
	}


	public BookAuthor? GetByAuthorId(Guid authorId, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_bookAuthors, filter).FirstOrDefault(ba => ba.AuthorId == authorId);
	}


	public IReadOnlyList<BookAuthor> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_bookAuthors, filter)];
	}


	public bool Exists<T>(T value, Func<BookAuthor, T> selector, Guid? excludeId)
	{
		return _bookAuthors.Any(ba =>
			!ba.Author.IsRemoved &&
			!ba.Book.IsRemoved &&
			(excludeId is null || ba.AuthorId != excludeId || ba.BookId != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(ba), value));
	}


	

	// ---------- Private helper ----------
	private static IEnumerable<BookAuthor> ApplyFilter(IEnumerable<BookAuthor> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(ba => !ba.Author.IsRemoved && !ba.Book.IsRemoved),
			EntityFilter.Removed => source.Where(ba => ba.Author.IsRemoved && ba.Book.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}