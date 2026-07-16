using System.Collections;

namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	private static int _nextUserId;
	private readonly List<UserRole> _userRoles = new();
	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; private set; }
	public DateOnly MembershipExpiryDate { get; private set; }


	public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();


	public User(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate, IEnumerable<Role> roles) : base(firstName, lastName, nationalCode, email, phoneNumber,
		birthDate)
	{
		Id = ++_nextUserId;
		IsActive = true;
		MembershipStartDate = DateOnly.FromDateTime(DateTime.Today);
		MembershipExpiryDate =
			MembershipStartDate.AddYears(1); // Should set a suitable end date based on business logic

		if (roles == null || !roles.Any())
		{
			throw new ArgumentException("A user must have at least one role.");
		}

		foreach (var role in roles)
		{
			_userRoles.Add(new UserRole
			{
				User = this,
				Role = role
			});
		}
	}



	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber,
		DateOnly? birthDate)
	{
		UpdateCore(firstName, lastName, nationalCode, email, phoneNumber, birthDate);
		User.ReplaceRoles(IEnumerable<Role> newRoles);
	}


	public void AssignRole(Role newRole)
	{
		if (!_userRoles.Any(ur => ur.RoleId == newRole.Id))
		{
			_userRoles.Add(new UserRole { User = this, Role = newRole });
		}
	}


	public void RemoveRole(int roleId)
	{
		var roleToRemove = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
		if (roleToRemove != null)
		{
			_userRoles.Remove(roleToRemove);
		}
	}


	public void ReplaceRoles(IEnumerable<Role> newRoles)
	{
		if (newRoles is null || !newRoles.Any())
		{
			throw new ArgumentException("A user must have at least one role.");
		}

		_userRoles.Clear();
		foreach (var role in newRoles)
		{
			_userRoles.Add(new UserRole { User = this, Role = role });
		}
	}
}