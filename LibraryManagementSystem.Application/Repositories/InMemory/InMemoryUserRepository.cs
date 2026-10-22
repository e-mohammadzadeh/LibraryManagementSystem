using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryUserRepository : IUserRepository
{
	private readonly List<User> _users = [];


	public void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		user.Id = Guid.CreateVersion7();
		user.CreatedAt = DateTime.UtcNow;
		user.IsActive = true;
		user.MembershipExpiryDate = user.MembershipStartDate.AddYears(1);
		user.IsRemoved = false;
		user.ShouldRemove = false;
		_users.Add(user);
	}


	public User? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u => u.Id == id);
	}


	public User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u =>
			u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter)
			.FirstOrDefault(u => u.Email.Equals(email));
	}


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_users, filter)];
	}


	public bool Exists<T>(T value, Func<User, T> selector, Guid? excludeId)
	{
		return _users.Any(a =>
			!a.IsRemoved &&
			(excludeId is null || a.Id != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(a), value));
	}


	public void Remove(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		if (user.IsRemoved) return;
		user.IsRemoved = true;
		user.IsActive = false;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = _users.FirstOrDefault(u => u.Id == user.Id) ?? throw new UserNotFoundException(user.Id);


		tracked.FirstName = user.FirstName;
		tracked.LastName = user.LastName;
		tracked.NationalCode = user.NationalCode;
		tracked.Email = user.Email;
		tracked.PhoneNumber = user.PhoneNumber;
		tracked.BirthDate = user.BirthDate;
		tracked.Role = user.Role;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = user.UpdatedByUserId;
	}


	// ---------- Private helper ---------
	private static IEnumerable<User> ApplyFilter(IEnumerable<User> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(b => !b.IsRemoved),
			EntityFilter.Removed => source.Where(b => b.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}