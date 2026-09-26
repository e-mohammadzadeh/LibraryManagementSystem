using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Domain.Entities;

public abstract class Person
{
	public Guid Id { get;  set; }
	public string FirstName { get;  set; } = string.Empty;
	public string LastName { get;  set; } = string.Empty;
	public string NationalCode { get;  set; } = string.Empty;
	public Email Email { get;  set; } = null!;
	public PhoneNumber PhoneNumber { get; set; } = null!;
	public DateOnly BirthDate { get;  set; }
	public DateTime CreatedAt { get;   set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }
}