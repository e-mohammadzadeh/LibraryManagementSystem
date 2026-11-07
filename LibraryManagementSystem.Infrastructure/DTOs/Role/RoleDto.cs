using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Role;

public class RoleDto
{
	public Guid Id { get; init; }
	public LibraryUserRole Name { get; init; }
}