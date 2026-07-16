using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role {
	public Role(LibraryUserRole name, string description)
	{
		Name = name;
		Description = description;
		Id = ++_nextRoleId;
		// Add AssignedDate - AssignedBy - CreatedAt
	}


	private static int _nextRoleId;
	public int Id { get; private set; }
	private readonly List<UserRole> _userRoles = []; 
	public LibraryUserRole Name { get; set; }
	public string Description { get; set; }


	internal void AddUserRole(UserRole userRole) {
		_userRoles.Add(userRole);
	}

	internal void RemoveUserRole(UserRole userRole) {
		_userRoles.Remove(userRole);
	}
}