using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryUserRepository : IUserRepository
{
	private readonly List<User> _users = [];


	public void Add(User user)
	{
		_users.Add(user);
	}


	public User? FindById(int id)
	{
		return _users.FirstOrDefault(u => u.Id == id);
	}


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


	public IReadOnlyList<User> GetAll()
	{
		return _users.AsReadOnly();
	}


	public bool ExistsByName(string firstName, string lastName, int excludeId = -1)
	{
		return _users.Any(u =>
			u.Id != excludeId && u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByNationalCode(string nationalCode, int excludeId = -1)
	{
		return _users.Any(u => u.Id != excludeId && u.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, int excludeId = -1)
	{
		return _users.Any(u => u.Id != excludeId && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1)
	{
		return _users.Any(u => u.Id != excludeId && u.PhoneNumber.Equals(phoneNumber));
	}


	public void Update(User user)
	{
		// Nothing to do.
		// The object reference has already been updated.
	}


	public void Remove(User user)
	{
		_users.Remove(user);
	}


	public IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return [];

		return
		[
			.. _users.Where(u =>
			{
				var value = selector(u);
				return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public IReadOnlyList<User> SearchByRole(List<int> roleIds)
	{
		ArgumentNullException.ThrowIfNull(roleIds);

		return [.. roleIds.Count == 0
			? []
			: _users.Where(u => u.UserRoles.Any(ur => roleIds.Contains(ur.RoleId)))];
	}
}