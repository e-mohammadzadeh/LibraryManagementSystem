using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class BookTranslatorRepository : IBookTranslatorRepository
{
	private readonly LibraryDbContext _context;


	public BookTranslatorRepository(LibraryDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}


	public IReadOnlyList<BookTranslator> GetByBookId(Guid id, EntityFilter filter)
	{
		var query = _context.Set<BookTranslator>()
			.AsNoTracking()
			.Include(bt => bt.Translator)
			.Where(bt => bt.BookId == id);

		query = filter switch
		{
			EntityFilter.Active => query.Where(bt => !bt.Translator.IsRemoved),
			EntityFilter.Removed => query.Where(bt => bt.Translator.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return [.. query];
	}


	public IReadOnlyList<BookTranslator> GetByTranslatorId(Guid id, EntityFilter filter)
	{
		var query = _context.Set<BookTranslator>()
			.AsNoTracking()
			.Include(bt => bt.Book).ThenInclude(b => b.BookAuthors).ThenInclude(x => x.Author)
			.Include(bt => bt.Book).ThenInclude(b => b.BookTranslators).ThenInclude(x => x.Translator)
			.AsSplitQuery()
			.Where(bt => bt.TranslatorId == id);

		query = filter switch
		{
			EntityFilter.Active => query.Where(bt => !bt.Book.IsRemoved),
			EntityFilter.Removed => query.Where(bt => bt.Book.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return [.. query.OrderBy(bt => bt.Book.Title)];
	}
}