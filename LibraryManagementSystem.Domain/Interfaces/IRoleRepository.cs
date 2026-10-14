using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAll();
	Role? FindById(Guid id);
	Role? FindByName(LibraryUserRole name);
}