using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class UserRepository : PersonRoleRepository<User>, IUserRepository
{
	public UserRepository(LibraryDbContext context) : base(context) { }


	public override void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (user.RoleId == Guid.Empty) throw new ArgumentException("RoleId cannot be empty.", nameof(user));

		var role = Context.Set<Role>().Find(user.RoleId) ??
		           throw new InvalidOperationException($"Role with Id '{user.RoleId}' was not found.");

		user.Role = role;
		base.Add(user);
	}



	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = GetTrackedById(user.Id);

		tracked.RoleId = user.RoleId;
		tracked.IsActive = user.IsActive;
		tracked.MembershipStartDate = user.MembershipStartDate;
		tracked.MembershipExpiryDate = user.MembershipExpiryDate;
		tracked.ShouldRemove = user.ShouldRemove;

		SaveRoleUpdate(tracked, user);
	}


	public override User? GetById(Guid id, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.Id == id);



	public override IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active) =>
		[.. Query(filter).OrderByDescending(u => u.CreatedAt)];


	public User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.Person.Email == email);


	public User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active) =>
		Query(filter).FirstOrDefault(u => u.Person.FirstName == firstName && u.Person.LastName == lastName);


	protected override Exception CreateNotFoundException(Guid id) => new UserNotFoundException(id);


	private IQueryable<User> Query(EntityFilter filter) =>
		ApplyFilter(Context.Set<User>()
			.AsNoTracking()
			.Include(u => u.Person)
			.Include(u => u.Role), filter);
}