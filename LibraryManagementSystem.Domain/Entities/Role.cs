using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
	public Guid Id { get;  set; }
	public LibraryUserRole Name { get; set; }
	public string? Description { get; set; }
}