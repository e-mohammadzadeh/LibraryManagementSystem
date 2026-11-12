using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class UserRepository : IUserRepository
{
	protected readonly LibraryDbContext Context;
	protected readonly DbSet<User> DbSet;


	public UserRepository(LibraryDbContext context)
	{
		Context = context;
		DbSet = context.Set<User>();
	}


	public void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		DbSet.Add(user);
		Context.SaveChanges();
	}


	public User? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(DbSet.AsNoTracking(), filter).FirstOrDefault(u => u.Id == id);
	}


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(DbSet.AsNoTracking(), filter)];
	}


	public User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(DbSet.AsNoTracking(), filter)
			.FirstOrDefault(u =>
				u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
				u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(DbSet.AsNoTracking(), filter)
			.FirstOrDefault(u => u.Email.Equals(email));
	}


	public bool Exists<TValue>(TValue value, Func<User, TValue> selector, Guid? excludeId)
	{
		ArgumentNullException.ThrowIfNull(selector);

		return DbSet
			.AsNoTracking()
			.Where(u => !u.IsRemoved)
			.AsEnumerable()
			.Any(u =>
				(excludeId == null || u.Id != excludeId) &&
				EqualityComparer<TValue>.Default.Equals(selector(u), value)
			);
	}


	public void Remove(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (user.IsRemoved) return;
		var tracked = DbSet.FirstOrDefault(u => u.Id == user.Id) ?? throw CreateNotFoundException(user.Id);
		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;

		Context.SaveChanges();
	}


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = DbSet.FirstOrDefault(u => u.Id == user.Id) ?? throw CreateNotFoundException(user.Id);

		tracked.FirstName = user.FirstName;
		tracked.LastName = user.LastName;
		tracked.NationalCode = user.NationalCode;
		tracked.Email = user.Email;
		tracked.PhoneNumber = user.PhoneNumber;
		tracked.BirthDate = user.BirthDate;
		tracked.Role = user.Role;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = user.UpdatedByUserId;

		Context.SaveChanges();
	}


	protected Exception CreateNotFoundException(Guid id) => new UserNotFoundException(id);


	private static IQueryable<User> ApplyFilter(IQueryable<User> query, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => query.Where(u => !u.IsRemoved),
			EntityFilter.Removed => query.Where(u => u.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}