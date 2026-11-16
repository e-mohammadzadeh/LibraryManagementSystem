using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class BookAuthorRepository : IBookAuthorRepository
{
	private readonly LibraryDbContext _context;


	public BookAuthorRepository(LibraryDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}


	public IReadOnlyList<BookAuthor> GetByBookId(Guid id, EntityFilter filter)
	{
		var query = _context.Set<BookAuthor>()
			.AsNoTracking()
			.Include(ba => ba.Author)
			.Where(ba => ba.BookId == id);

		query = filter switch
		{
			EntityFilter.Active => query.Where(ba => !ba.Author.IsRemoved),
			EntityFilter.Removed => query.Where(ba => ba.Author.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return [.. query];
	}


	public IReadOnlyList<BookAuthor> GetByAuthorId(Guid id, EntityFilter filter)
	{
		var query = _context.Set<BookAuthor>()
			.AsNoTrackingWithIdentityResolution()
			.Include(ba => ba.Book).ThenInclude(b => b.BookAuthors).ThenInclude(x => x.Author)
			.Include(ba => ba.Book).ThenInclude(b => b.BookTranslators).ThenInclude(x => x.Translator)
			.AsSplitQuery()
			.Where(ba => ba.AuthorId == id);

		query = filter switch
		{
			EntityFilter.Active => query.Where(ba => !ba.Book.IsRemoved),
			EntityFilter.Removed => query.Where(ba => ba.Book.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return [.. query.OrderBy(ba => ba.Book.Title)];
	}
}