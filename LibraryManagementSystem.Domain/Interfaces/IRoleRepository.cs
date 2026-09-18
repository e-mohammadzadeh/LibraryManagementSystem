using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAllRoles();
	IReadOnlyList<Role> FindByIds(IEnumerable<Guid> ids);
}