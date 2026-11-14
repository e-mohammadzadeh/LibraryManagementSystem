using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class UserRepository : IUserRepository
{
	protected readonly LibraryDbContext Context;


	public UserRepository(LibraryDbContext context) => Context = context;



	private IQueryable<User> Query(EntityFilter filter) =>
		ApplyFilter(Context.Users.AsNoTracking().Include(u => u.Role), filter);


	public void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (user.RoleId == Guid.Empty && user.Role is not null)
			user.RoleId = user.Role.Id;

		var role = user.Role;
		user.Role = null!;

		user.Id = Guid.CreateVersion7();
		user.CreatedAt = DateTime.UtcNow;

		Context.Users.Add(user);
		Context.SaveChanges();
		Context.Entry(user).State = EntityState.Detached;

		user.Role = role;
	}


	public User? GetById(Guid id, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.Id == id);


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active) =>
		[.. Query(filter).OrderByDescending(u => u.CreatedAt)];


	public User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);


	public User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.Email == email);


	public bool Exists(Expression<Func<User, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		return Context.Users
			.AsNoTracking()
			.Where(u => !u.IsRemoved)
			.Where(predicate)
			.Any(u => excludeId == null || u.Id != excludeId);
	}


	public void Remove(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = Context.Users.FirstOrDefault(u => u.Id == user.Id) ?? throw CreateNotFoundException(user.Id);
		if (tracked.IsRemoved) return;

		tracked.IsRemoved = true;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = user.UpdatedByUserId;

		Context.SaveChanges();
	}


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = Context.Users.FirstOrDefault(u => u.Id == user.Id) ?? throw CreateNotFoundException(user.Id);

		tracked.FirstName = user.FirstName;
		tracked.LastName = user.LastName;
		tracked.NationalCode = user.NationalCode;
		tracked.Email = user.Email;
		tracked.PhoneNumber = user.PhoneNumber;
		tracked.BirthDate = user.BirthDate;

		tracked.IsActive = user.IsActive;
		tracked.MembershipStartDate = user.MembershipStartDate;
		tracked.MembershipExpiryDate = user.MembershipExpiryDate;
		tracked.ShouldRemove = user.ShouldRemove;

		tracked.PasswordHash = user.PasswordHash;
		tracked.PasswordSalt = user.PasswordSalt;
		tracked.LastLoginDate = user.LastLoginDate;
		tracked.PreviousLoginDate = user.PreviousLoginDate;

		tracked.RoleId = user.RoleId;

		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = user.UpdatedByUserId;

		Context.SaveChanges();
	}


	private static Exception CreateNotFoundException(Guid id) => new UserNotFoundException(id);


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