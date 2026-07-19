using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAllRoles();
	IReadOnlyList<Role> FindByIds(IEnumerable<int> ids);
	bool RoleExists(int id);
	int Count();
}