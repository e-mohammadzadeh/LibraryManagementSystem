using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	private static int _nextUserId;
	private readonly List<UserRole> _userRoles = [];
	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; set; }
	public DateOnly MembershipExpiryDate { get; private set; }
	public bool ShouldRemove { get; private set; }
	public byte[] PasswordHash { get; private set; }
	public byte[] PasswordSalt { get; private set; }
	public DateTime? LastLoginDate { get; private set; }


	public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();


	public User(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, IEnumerable<Role> roles, string password, IPasswordHasher passwordHasher,
		DateOnly? membershipStartDate = null) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextUserId;
		IsActive = true;
		MembershipStartDate = membershipStartDate ?? DateOnly.FromDateTime(DateTime.Today);
		// Should set a suitable end date based on business logic
		MembershipExpiryDate = MembershipStartDate.AddYears(1);
		ShouldRemove = false;
		if (roles == null || !roles.Any())
		{
			throw new ArgumentException("A user must have at least one role.");
		}

		var rolesList = roles.ToList();
		if (rolesList.Count == 0) throw new ArgumentException("A user must have at least one role.");
		foreach (var role in rolesList) AssignRole(role);

		passwordHasher.CreatePasswordHash(password, out var hash, out var salt);
		SetPasswordHash(hash, salt);
	}



	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate, IEnumerable<Role>? roles)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
		if (roles is not null)
		{
			ReplaceRoles(roles);
		}
	}


	public void AssignRole(Role role)
	{
		if (role is null) throw new ArgumentNullException(nameof(role));

		// Prevent duplicate roles
		if (_userRoles.Any(ur => ur.Role.Id == role.Id)) return;

		var userRole = new UserRole(this, role);

		_userRoles.Add(userRole);
		role.AddUserRole(userRole);
	}


	public void RemoveRole(int roleId)
	{
		if (_userRoles.Count == 1) throw new InvalidOperationException("A user must have at least one role.");

		var userRole = _userRoles.FirstOrDefault(ur => ur.Role.Id == roleId);

		if (userRole is null) return;

		_userRoles.Remove(userRole);
		userRole.Role.RemoveUserRole(userRole);
	}


	public void ReplaceRoles(IEnumerable<Role> newRoles)
	{
		if (newRoles is null) throw new ArgumentNullException(nameof(newRoles));

		var roles = newRoles.DistinctBy(r => r.Id).ToList();

		if (roles.Count == 0) throw new ArgumentException("A user must have at least one role.");

		// remove old roles correctly
		foreach (var userRole in _userRoles.ToList())
		{
			_userRoles.Remove(userRole);
			userRole.Role.RemoveUserRole(userRole);
		}

		// add new roles correctly
		foreach (var role in roles)
		{
			AssignRole(role);
		}
	}


	public void FlagForRemoval() { ShouldRemove = true; }


	public void SetPasswordHash(byte[] passwordHash, byte[] passwordSalt)
	{
		PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
		PasswordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
	}


	public void UpdateLastLogin() { LastLoginDate = DateTime.Now; }
}