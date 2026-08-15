using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
	public Role(LibraryUserRole name, string description)
	{
		Name = name;
		Description = description;
		Id = ++_nextRoleId;
	}


	private static int _nextRoleId;
	private readonly List<UserRole> _userRoles = [];
	public int Id { get; private set; }
	public LibraryUserRole Name { get; set; }
	public string Description { get; set; }


	internal void AddUserRole(UserRole userRole)
	{
		ArgumentNullException.ThrowIfNull(userRole);
		if (_userRoles.Contains(userRole)) return;
		_userRoles.Add(userRole);
	}


	internal void RemoveUserRole(UserRole userRole)
	{
		ArgumentNullException.ThrowIfNull(userRole);
		_userRoles.Remove(userRole);
	}

	public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
}