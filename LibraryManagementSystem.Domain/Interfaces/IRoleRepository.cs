using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAllRoles();
	Role? GetRoleById(int id);
	bool RoleExists(int id);
}