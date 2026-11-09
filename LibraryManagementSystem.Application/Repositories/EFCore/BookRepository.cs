using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class BookRepository : IBookRepository
{
	protected readonly LibraryDbContext Context;
	protected readonly DbSet<Book> DbSet;


	public BookRepository(LibraryDbContext context)
	{
		Context = context;
		DbSet = context.Set<Book>();
	}


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		DbSet.Add(book);
		Context.SaveChanges();
	}


	public Book? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(DbSet.AsNoTracking(), filter).FirstOrDefault(b => b.Id == id);
	}


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(DbSet.AsNoTracking(), filter)];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return
		[
			.. ApplyFilter(DbSet.AsNoTracking(), filter)
				.Where(b => b.BookAuthors
					.Any(a => a.AuthorId == id)
				)
		];
	}


	public IReadOnlyList<Book> GetByTranslatorId(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return
		[
			.. ApplyFilter(DbSet.AsNoTracking(), filter)
				.Where(bt => bt.BookTranslators
					.Any(t => t.TranslatorId == id)
				)
		];
	}


	public bool Exists<TValue>(TValue value, Func<Book, TValue> selector, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(selector);

		return DbSet
			.AsNoTracking()
			.Where(b => !b.IsRemoved)
			.AsEnumerable()
			.Any(b =>
				(excludeId == null || b.Id != excludeId) &&
				EqualityComparer<TValue>.Default.Equals(selector(b), value)
			);
	}


	public void Remove(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (book.IsRemoved) return;
		var tracked = DbSet.FirstOrDefault(b => b.Id == book.Id) ?? throw CreateNotFoundException(book.Id);
		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;

		Context.SaveChanges();
	}


	public void Update(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		var tracked = DbSet
			.Include(b => b.BookAuthors)
			.Include(b => b.BookTranslators)
			.FirstOrDefault(b => b.Id == book.Id)
		              ?? throw CreateNotFoundException(book.Id);

		tracked.Title = book.Title;
		tracked.ISBN = book.ISBN;
		UpdateAuthors(tracked, book);
		UpdateTranslators(tracked, book);
		tracked.PublishDate = book.PublishDate;
		tracked.Genre = book.Genre;
		tracked.Publisher = book.Publisher;
		tracked.OriginalLanguage = book.OriginalLanguage;
		tracked.TotalCopies = book.TotalCopies;
		tracked.AvailableCopies = book.AvailableCopies;
		tracked.Description = book.Description;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = book.UpdatedByUserId;

		Context.SaveChanges();
	}


	protected Exception CreateNotFoundException(Guid id) => new BookNotFoundException(id);


	private static IQueryable<Book> ApplyFilter(IQueryable<Book> query, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => query.Where(b => !b.IsRemoved),
			EntityFilter.Removed => query.Where(b => b.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}


	private void UpdateAuthors(Book tracked, Book updated)
	{
		tracked.BookAuthors.Clear();

		foreach (var bookAuthor in updated.BookAuthors)
		{
			tracked.BookAuthors.Add(new BookAuthor
			{
				BookId = tracked.Id,
				AuthorId = bookAuthor.AuthorId
			});
		}
	}


	private void UpdateTranslators(Book tracked, Book updated)
	{
		tracked.BookTranslators.Clear();

		foreach (var bookTranslator in updated.BookTranslators)
		{
			tracked.BookTranslators.Add(new BookTranslator
			{
				BookId = tracked.Id,
				TranslatorId = bookTranslator.TranslatorId
			});
		}
	}
}