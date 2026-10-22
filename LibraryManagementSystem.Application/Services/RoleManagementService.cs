using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Services;

public class RoleManagementService
{
	private readonly IRoleRepository _roleRepository;


	public RoleManagementService(IRoleRepository roleRepository) { _roleRepository = roleRepository; }

	public IReadOnlyList<Role> GetAllRoles() { return _roleRepository.GetAll(); }

	public Role? GetById(Guid id) => _roleRepository.GetById(id);
}