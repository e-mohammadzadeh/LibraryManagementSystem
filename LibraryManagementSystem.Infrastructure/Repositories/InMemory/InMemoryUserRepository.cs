using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryUserRepository : IUserRepository
{
	private readonly List<User> _users = [];


	public void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		_users.Add(user);
	}


	public User? FindById(Guid id) { return _users.FirstOrDefault(u => u.Id == id); }


	public User? FindByName(string firstName, string lastName)
	{
		return _users.FirstOrDefault(u =>
			u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public User? FindByEmail(string email)
	{
		return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		var query = _users.AsEnumerable();

		switch (filter)
		{
			case EntityFilter.Active:
				query = query.Where(u => !u.IsRemoved);
				break;
			case EntityFilter.Removed:
				query = query.Where(u => u.IsRemoved);
				break;
			case EntityFilter.All:
				// No filter – include everyone
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
		}

		return [.. query];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.PhoneNumber.Equals(phoneNumber));
	}


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		var existingUserIndex = _users.FindIndex(u => u.Id == user.Id);
		if (existingUserIndex == -1) throw new KeyNotFoundException($"User with ID {user.Id} was not found.");
		_users[existingUserIndex] = user;
		user.UpdatedAt = DateTime.Now;
	}


	public void Remove(User user) { user.DeleteUser(); }


	public IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];

		return
		[
			.. _users.Where(u =>
			{
				var value = selector(u);
				return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public IReadOnlyList<User> SearchByRole(IReadOnlyList<int> roleIds)
	{
		ArgumentNullException.ThrowIfNull(roleIds);

		return
		[
			.. roleIds.Count == 0
				? []
				: _users.Where(u => u.UserRoles.Any(ur => roleIds.Contains(ur.RoleId)))
		];
	}
}