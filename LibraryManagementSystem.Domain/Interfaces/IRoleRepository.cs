using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IRoleRepository
{
	IReadOnlyList<Role> GetAll();
	Role? GetById(Guid id);
	Role? GetByName(LibraryUserRole name);
}