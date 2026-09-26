using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
	public Guid Id { get;  set; }
	public LibraryUserRole Name { get; set; }
}