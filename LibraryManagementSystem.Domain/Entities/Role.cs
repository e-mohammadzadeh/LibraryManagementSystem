using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role {
	public Role(string description)
	{
		Description = description;
		RoleId = ++_nextRoleId;
	}


	private static int _nextRoleId;
	public int RoleId { get; set; }
	public LibraryUserRole Name { get; set; }
	public string Description { get; set; }
}