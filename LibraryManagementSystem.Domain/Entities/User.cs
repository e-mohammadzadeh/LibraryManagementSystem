namespace LibraryManagementSystem.Domain.Entities;

public class User : Person
{
	private static int _nextUserId;
	private readonly List<UserRole> _userRoles = [];
	public bool IsActive { get; private set; }
	public DateOnly MembershipStartDate { get; private set; }
	public DateOnly MembershipExpiryDate { get; private set; }


	public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();


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
			AssignRole(role);
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
		if (role is null)
		{
			throw new ArgumentNullException(nameof(role));
		}

		// Prevent duplicate roles
		if (_userRoles.Any(ur => ur.Role.Id == role.Id))
			return;

		var userRole = new UserRole(this, role);

		_userRoles.Add(userRole);
		role.AddUserRole(userRole);
	}


	public void RemoveRole(int roleId)
	{
		if (_userRoles.Count == 1)
			throw new InvalidOperationException("A user must have at least one role.");

		var userRole = _userRoles.FirstOrDefault(ur => ur.Role.Id == roleId);

		if (userRole is null)
			return;

		_userRoles.Remove(userRole);
		userRole.Role.RemoveUserRole(userRole);
	}


	public void ReplaceRoles(IEnumerable<Role> newRoles)
	{
		if (newRoles is null)
		{
			throw new ArgumentNullException(nameof(newRoles));
		}

		var roles = newRoles.DistinctBy(r => r.Id).ToList();

		if (roles.Count == 0)
			throw new ArgumentException("A user must have at least one role.");

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
}