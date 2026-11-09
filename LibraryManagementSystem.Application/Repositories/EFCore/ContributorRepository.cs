using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public abstract class ContributorRepository<T> : IContributorRepository<T> where T : Contributor
{
	protected readonly LibraryDbContext Context;
	protected readonly DbSet<T> DbSet;


	protected ContributorRepository(LibraryDbContext context)
	{
		Context = context;
		DbSet = context.Set<T>();
	}


	public void Add(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		DbSet.Add(entity);
		Context.SaveChanges();
	}


	public T? GetById(Guid id, EntityFilter filter)
	{
		return ApplyFilter(DbSet.AsNoTracking(), filter).FirstOrDefault(c => c.Id == id);
	}


	public IReadOnlyList<T> GetAll(EntityFilter filter) { return [.. ApplyFilter(DbSet.AsNoTracking(), filter)]; }


	public bool Exists<TValue>(TValue value, Func<T, TValue> selector, Guid? excludeId = null)
	{
		// TODO need some improvements based on database
		ArgumentNullException.ThrowIfNull(selector);

		return DbSet
			.AsNoTracking()
			.Where(c => !c.IsRemoved)
			.AsEnumerable()
			.Any(c =>
				(excludeId == null || c.Id != excludeId) && 
				EqualityComparer<TValue>.Default.Equals(selector(c), value)
				);
	}


	public void Remove(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		if (entity.IsRemoved) return;
		var tracked = DbSet.FirstOrDefault(c => c.Id == entity.Id) ?? throw CreateNotFoundException(entity.Id);

		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;

		Context.SaveChanges();
	}


	public void Update(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		var tracked = DbSet.FirstOrDefault(c => c.Id == entity.Id) ?? throw CreateNotFoundException(entity.Id);

		tracked.FirstName = entity.FirstName;
		tracked.LastName = entity.LastName;
		tracked.NationalCode = entity.NationalCode;
		tracked.Email = entity.Email;
		tracked.PhoneNumber = entity.PhoneNumber;
		tracked.BirthDate = entity.BirthDate;
		tracked.Biography = entity.Biography;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = entity.UpdatedByUserId;

		Context.SaveChanges();
	}


	protected abstract Exception CreateNotFoundException(Guid id);


	private static IQueryable<T> ApplyFilter(IQueryable<T> query, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => query.Where(c => !c.IsRemoved),
			EntityFilter.Removed => query.Where(c => c.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}