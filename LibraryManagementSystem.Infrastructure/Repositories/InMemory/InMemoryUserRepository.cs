using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryUserRepository : IUserRepository
{
	private readonly List<User> _users = new();


	public void Add(User user)
	{
		_users.Add(user);
	}


	public User? FindById(int id)
	{
		return _users.FirstOrDefault(m => m.Id == id);
	}


	public User? FindByName(string firstName, string lastName)
	{
		return _users.FirstOrDefault(user =>
			user.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			user.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<User> GetAll()
	{
		return _users.AsReadOnly();
	}


	public bool ExistsByName(string firstName, string lastName, int excludeId = -1)
	{
		return _users.Any(user =>
			user.Id != excludeId && user.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			user.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByNationalCode(string nationalCode, int excludeId = -1)
	{
		return _users.Any(user => user.Id != excludeId && user.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, int excludeId = -1)
	{
		return _users.Any(user => user.Id != excludeId && user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1)
	{
		return _users.Any(user => user.Id != excludeId && user.PhoneNumber.Equals(phoneNumber));
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


	public IReadOnlyList<User> Search(string searchItem, Func<User, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<User>();

		return _users.Where(member =>
		{
			var value = selector(member);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}
}