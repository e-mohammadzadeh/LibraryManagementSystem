using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public abstract class PersonRoleRepository<T> : IPersonRoleRepository<T> where T : class, IPersonRoleEntity
{
	protected readonly LibraryDbContext Context;


	protected PersonRoleRepository(LibraryDbContext context) { Context = context; }


	public virtual void Add(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		if (entity.PersonId == Guid.Empty) throw new ArgumentException("PersonId cannot be empty", nameof(entity));

		var person = Context.Set<Person>().Find(entity.PersonId) ??
		             throw new InvalidOperationException($"Person with ID '{entity.PersonId}' was not found.");

		entity.Person = person;
		entity.Id = Guid.CreateVersion7();
		entity.CreatedAt = DateTime.UtcNow;
		entity.IsRemoved = false;

		Context.Set<T>().Add(entity);
		Context.SaveChanges();
	}


	public virtual T? GetById(Guid id, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(e => e.Id == id);


	public T? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.All) =>
		Query(filter).FirstOrDefault(e => e.PersonId == personId);


	public virtual IReadOnlyList<T> GetAll(EntityFilter filter = EntityFilter.Active) =>
		[.. Query(filter).OrderByDescending(e => e.CreatedAt)];


	public void Remove(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);

		var tracked = GetTrackedById(entity.Id);
		if (tracked.IsRemoved) return;

		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = entity.UpdatedByUserId;

		Context.SaveChanges();
	}


	public virtual void Restore(T entity, Guid? updatedByUserId = null)
	{
		ArgumentNullException.ThrowIfNull(entity);

		var tracked = GetTrackedById(entity.Id);
		if (!tracked.IsRemoved) return;

		tracked.IsRemoved = false;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = updatedByUserId;

		Context.SaveChanges();
	}


	protected virtual IQueryable<T> Query(EntityFilter filter) =>
		ApplyFilter(Context.Set<T>().AsNoTracking().Include(e => e.Person), filter);



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


	protected static IQueryable<T> ApplyFilter(IQueryable<T> query, EntityFilter filter)
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