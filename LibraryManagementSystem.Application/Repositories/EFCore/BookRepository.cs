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


	public BookRepository(LibraryDbContext context) => Context = context;


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (book.Id == Guid.Empty) book.Id = Guid.CreateVersion7();
		book.AvailableCopies = book.TotalCopies;
		book.CreatedAt = DateTime.UtcNow;
		book.IsRemoved = false;

		Context.Books.Add(book);
		Context.SaveChanges();
		Context.ChangeTracker.Clear();
	}


	public Book? GetById(Guid id, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(b => b.Id == id);


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active) =>
		[.. Query(filter).OrderByDescending(b => b.CreatedAt)];



	public IReadOnlyList<Book> GetByAuthorId(Guid id, EntityFilter filter = EntityFilter.Active) =>
		[.. Query(filter).Where(b => b.BookAuthors.Any(a => a.AuthorId == id))];



	public IReadOnlyList<Book> GetByTranslatorId(Guid id, EntityFilter filter = EntityFilter.Active) =>
	[
		.. Query(filter).Where(bt => bt.BookTranslators.Any(t => t.TranslatorId == id))
	];



	public bool Exists(Expression<Func<Book, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		var query = Context.Books
			.AsNoTracking()
			.Where(b => !b.IsRemoved)
			.Where(predicate);

		if (excludeId.HasValue) query = query.Where(b => b.Id != excludeId.Value);
		return query.Any();
	}


	public bool ExistsIncludingRemoved(Expression<Func<Book, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		var query = Context.Books
			.AsNoTracking()
			.Where(predicate);

		if (excludeId.HasValue) query = query.Where(b => b.Id != excludeId.Value);

		return query.Any();
	}


	public void Remove(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		var tracked = Context.Books.FirstOrDefault(b => b.Id == book.Id) ?? throw CreateNotFoundException(book.Id);
		if (tracked.IsRemoved) return;

		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = book.UpdatedByUserId;

		Context.SaveChanges();
		Context.ChangeTracker.Clear();
	}


	public void Update(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		var tracked = Context.Books
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
		Context.ChangeTracker.Clear();
	}


	private static BookNotFoundException CreateNotFoundException(Guid id) => new(id);


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
		var incomingTranslators = updated.BookTranslators
			.ToDictionary(bt => bt.TranslatorId);

		var relationshipsToRemove = tracked.BookTranslators
			.Where(bt => !incomingTranslators.ContainsKey(bt.TranslatorId))
			.ToList();

		Context.Set<BookTranslator>().RemoveRange(relationshipsToRemove);

		foreach (var existing in tracked.BookTranslators)
			if (incomingTranslators.TryGetValue(existing.TranslatorId, out var incoming))
				existing.TranslationLanguage = incoming.TranslationLanguage;

		var existingIds = tracked.BookTranslators
			.Select(bt => bt.TranslatorId)
			.ToHashSet();

		foreach (var incoming in incomingTranslators.Values)
		{
			if (existingIds.Contains(incoming.TranslatorId)) continue;

			tracked.BookTranslators.Add(new BookTranslator
			{
				BookId = tracked.Id,
				TranslatorId = incoming.TranslatorId,
				TranslationLanguage = incoming.TranslationLanguage
			});
		}
	}


	private IQueryable<Book> Query(EntityFilter filter) =>
		ApplyFilter(Context.Books
			.AsNoTracking()
			.Include(b => b.BookAuthors)
			.ThenInclude(ba => ba.Author)
			.Include(b => b.BookTranslators)
			.ThenInclude(bt => bt.Translator)
			.AsSplitQuery(), filter);
}