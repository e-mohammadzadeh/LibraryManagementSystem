using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class RoleRepository : IRoleRepository
{
	protected readonly LibraryDbContext Context;


	public RoleRepository(LibraryDbContext context) { Context = context; }


	public IReadOnlyList<Role> GetAll() => [.. Context.Roles.AsNoTracking()];
	public Role? GetById(Guid id) => Context.Roles.AsNoTracking().FirstOrDefault(r => r.Id == id);
	public Role? GetByName(LibraryUserRole name) => Context.Roles.AsNoTracking().FirstOrDefault(r => r.Name == name);
}