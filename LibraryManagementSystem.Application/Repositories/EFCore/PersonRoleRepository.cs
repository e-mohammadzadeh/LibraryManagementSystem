using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public abstract class PersonRoleRepository<T> : IPersonRoleRepository<T> where T : class, IPersonRoleEntity
{
	protected readonly LibraryDbContext Context;


	public PersonRoleRepository(LibraryDbContext context) { Context = context; }


	public void Add(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		if (entity.PersonId == Guid.Empty) throw new ArgumentException("PersonId is required");

		entity.Id = Guid.CreateVersion7();
		entity.CreatedAt = DateTime.UtcNow;

		Context.Set<T>().Add(entity);
		Context.SaveChanges();
	}


	public T? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(Context.Set<T>()
				.AsNoTracking()
				.Include("Person"), filter)
			.FirstOrDefault(e => e.Id == id);
	}


	public IReadOnlyList<T> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return
		[
			.. ApplyFilter(Context.Set<T>()
					.AsNoTracking()
					.Include("Person"), filter)
				.OrderByDescending(e => e.CreatedAt)
		];
	}


	public void Remove(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		var tracked = Context.Set<T>().FirstOrDefault(e => e.Id == entity.Id) ??
		              throw CreateNotFoundException(entity.Id);

		if (tracked.IsRemoved) return;

		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = entity.UpdatedByUserId;

		Context.SaveChanges();
	}


	protected T GetTrackedById(Guid id)
	{
		return Context.Set<T>().FirstOrDefault(e => e.Id == id) ?? throw CreateNotFoundException(id);
	}


	protected void SaveRoleUpdate(IPersonRoleEntity tracked, IPersonRoleEntity updated)
	{
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = updated.UpdatedByUserId;

		Context.SaveChanges();
	}


	protected abstract Exception CreateNotFoundException(Guid id);


	private static IQueryable<T> ApplyFilter(IQueryable<T> query, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => query.Where(e => !e.IsRemoved),
			EntityFilter.Removed => query.Where(e => e.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}