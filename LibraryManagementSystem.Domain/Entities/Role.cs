using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
	public Role(LibraryUserRole name, string description)
	{
		Id = Guid.CreateVersion7();
		Name = name;
		Description = description;
	}

	public Guid Id { get;  set; }
	public LibraryUserRole Name { get; }
	public string Description { get; private set; }
}