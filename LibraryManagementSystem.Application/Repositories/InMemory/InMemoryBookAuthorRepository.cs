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

		if (alreadyExists)
			throw new InvalidOperationException("This author is already associated with this book.");

		_bookAuthors.Add(bookAuthor);
	}


	public void Remove(BookAuthor bookAuthor)
	{
		ArgumentNullException.ThrowIfNull(bookAuthor);


	}


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


	public IReadOnlyList<BookAuthor> GetByBookId(Guid bookId, EntityFilter filter = EntityFilter.Active)
	{
		return 
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