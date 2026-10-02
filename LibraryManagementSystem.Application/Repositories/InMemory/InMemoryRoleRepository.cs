using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryRoleRepository : IRoleRepository
{
	private readonly List<Role> _roles =
	[
		new() { Id = Guid.CreateVersion7(), Name = LibraryUserRole.Member },
		new() { Id = Guid.CreateVersion7(), Name = LibraryUserRole.Librarian },
		new() { Id = Guid.CreateVersion7(), Name = LibraryUserRole.Admin }
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