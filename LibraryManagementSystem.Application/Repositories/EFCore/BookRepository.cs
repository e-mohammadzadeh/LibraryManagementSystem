using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

		book.Id = Guid.CreateVersion7();
		book.AvailableCopies = book.TotalCopies;
		book.CreatedAt = DateTime.UtcNow;
		book.IsRemoved = false;

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


	public bool Exists<TValue>(Expression<Func<Book, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		return DbSet
			.AsNoTracking()
			.Where(b => !b.IsRemoved)
			.Where(predicate)
			.Any(b => excludeId == null || b.Id != excludeId);
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
		var incomingIds = updated.BookAuthors
			.Select(ba => ba.AuthorId)
			.ToHashSet();

		var existingIds = tracked.BookAuthors
			.Select(ba => ba.AuthorId)
			.ToHashSet();

		var toRemove = tracked.BookAuthors.Where(ba => !incomingIds.Contains(ba.AuthorId)).ToList();

		Context.Set<BookAuthor>().RemoveRange(toRemove);

		foreach (var authorId in incomingIds.Except(existingIds))
		{
			tracked.BookAuthors.Add(new BookAuthor
			{
				BookId = tracked.Id,
				AuthorId = authorId
			});
		}
	}


	private void UpdateTranslators(Book tracked, Book updated)
	{
		var incomingIds = updated.BookTranslators
			.Select(bt => bt.TranslatorId)
			.ToHashSet();

		var existingIds = tracked.BookTranslators
			.Select(bt => bt.TranslatorId)
			.ToHashSet();

		var toRemove = tracked.BookTranslators.Where(bt => !incomingIds.Contains(bt.TranslatorId)).ToList();

		Context.Set<BookTranslator>().RemoveRange(toRemove);

		foreach (var translatorId in incomingIds.Except(existingIds))
		{
			tracked.BookTranslators.Add(new BookTranslator
			{
				BookId = tracked.Id,
				TranslatorId = translatorId
			});
		}
	}
}