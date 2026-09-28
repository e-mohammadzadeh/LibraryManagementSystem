using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

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
		user.IsRemoved = false;
		_users.Add(user);
	}


	public User? FindById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u => u.Id == id);
	}


	public User? FindByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u =>
			u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public User? FindByEmail(Email email, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter)
			.FirstOrDefault(u => u.Email.Equals(email));
	}


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_users, filter)];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(Email email, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.Email.Equals(email));
	}


	public bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId = null)
	{
		return _users.Any(u => u.Id != excludeId && u.PhoneNumber.Equals(phoneNumber));
	}


	public void Update(User user, Guid? updatedBy = null)
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
		tracked.RoleId = user.RoleId;
		tracked.UpdatedAt = DateTime.UtcNow;
	}


	public void Remove(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		if (user.IsRemoved) return;
		user.IsRemoved = true;
		user.IsActive = false;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<User> Search(string searchTerm, Func<User, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];

		return
		[
			.. _users
				.Where(u => !u.IsRemoved)
				.Where(u =>
				{
					var value = selector(u);
					return value is not null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
				})
		];
	}


	public IReadOnlyList<User> SearchByRole(Guid roleIds)
	{
		return [.. _users.Where(u => !u.IsRemoved).Where(u => u.RoleId == roleIds)];
	}


	public void ReplaceRole(User user, Role newRole)
	{
		user.Role = newRole ?? throw new ArgumentNullException(nameof(newRole));
		user.RoleId = newRole.Id;
	}


	public void RenewMembership(User user, int years = 1)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		var renewalBase = user.MembershipExpiryDate > today
			? user.MembershipExpiryDate // extend from current expiry if not yet expired
			: today; // restart from today if already expired

		user.MembershipExpiryDate = renewalBase.AddYears(years);
		if (!user.IsActive) user.IsActive = true;
		user.UpdatedAt = DateTime.UtcNow;
	}



	public void FlagForRemoval(User user)
	{
		user.ShouldRemove = true;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void SetPasswordHash(User user, byte[] passwordHash, byte[] passwordSalt)
	{
		if (passwordHash is null || passwordHash.Length == 0) throw new ArgumentNullException(nameof(passwordHash));
		if (passwordSalt is null || passwordSalt.Length == 0) throw new ArgumentNullException(nameof(passwordSalt));

		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;
	}


	public void UpdateLastLogin(User user)
	{
		user.LastLoginDate = user.PreviousLoginDate;
		user.PreviousLoginDate = DateTime.Now;
	}


	public void UpdateLastLoginInLogout(User user) { user.LastLoginDate = user.PreviousLoginDate; }



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