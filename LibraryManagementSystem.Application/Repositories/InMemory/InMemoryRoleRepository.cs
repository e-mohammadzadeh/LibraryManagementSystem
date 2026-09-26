using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

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


	public Role? FindById(Guid id)
	{
		return _roles.FirstOrDefault(r => id == r.Id);
	}
}