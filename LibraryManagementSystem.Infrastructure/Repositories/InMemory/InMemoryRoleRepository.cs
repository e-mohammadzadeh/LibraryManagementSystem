using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryRoleRepository : IRoleRepository
{
	private readonly List<Role> _roles =
	[
		new(LibraryUserRole.Member, "Can borrow books"),
		new(LibraryUserRole.Librarian, "Can manage books and members"),
		new(LibraryUserRole.Admin, "Full system access")
	];

	public IReadOnlyList<Role> GetAllRoles()
	{
		return _roles.AsReadOnly();
	}


	public IReadOnlyList<Role> FindByIds(IEnumerable<int> ids)
	{
		return [.. _roles.Where(role => ids.Contains(role.Id))];
	}
}