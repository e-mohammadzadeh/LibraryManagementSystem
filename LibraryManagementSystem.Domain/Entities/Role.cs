using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
	public Role(LibraryUserRole name, string description)
	{
		Id = Guid.CreateVersion7();
		Name = name;
		Description = description;
	}


	private readonly List<UserRole> _userRoles = [];
	public Guid Id { get;  set; }
	public LibraryUserRole Name { get; }
	public string Description { get; private set; }


	internal void AddUserRole(UserRole userRole)
	{
		ArgumentNullException.ThrowIfNull(userRole);
		if (_userRoles.Any(ur => ur.UserId == userRole.UserId)) return;
		_userRoles.Add(userRole);
	}


	internal void RemoveUserRole(UserRole userRole)
	{
		ArgumentNullException.ThrowIfNull(userRole);
		_userRoles.Remove(userRole);
	}

	public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
}