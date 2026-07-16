using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAllRoles();
	IReadOnlyList<Role> GetRolesByIds(IEnumerable<int> ids);
	bool RoleExists(int id);
}