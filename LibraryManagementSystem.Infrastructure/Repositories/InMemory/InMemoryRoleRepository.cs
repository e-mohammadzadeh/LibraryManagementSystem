using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryRoleRepository : IRoleRepository
{
	private readonly List<Role> _roles = new()
	{
		new Role(LibraryUserRole.Member, "Can borrow books"),
		new Role(LibraryUserRole.Librarian, "Can manage books and members"),
		new Role(LibraryUserRole.Admin, "Full system access")
	};

	public IReadOnlyList<Role> GetAllRoles()
	{
		return _roles.AsReadOnly();
	}


	public Role? GetRoleById(int id)
	{
		return _roles.FirstOrDefault(role => role.Id == id);
	}


	public bool RoleExists(int id)
	{
		return _roles.Any(role => role.Id == id);
	}
}