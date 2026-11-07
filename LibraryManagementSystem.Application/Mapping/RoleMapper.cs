using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Role;

namespace LibraryManagementSystem.Application.Mapping;

public static class RoleMapper
{
	public static RoleDto ToDto(this Role role)
	{
		return new RoleDto
		{
			Id = role.Id,
			Name = role.Name
		};
	}
}