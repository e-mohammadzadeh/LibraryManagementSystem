namespace LibraryManagementSystem.Domain.Entities;

public abstract class Person
{
	public Guid Id { get;  set; }
	public string FirstName { get;  set; } = string.Empty;
	public string LastName { get;  set; } = string.Empty;
	public string NationalCode { get;  set; } = string.Empty;
	public string Email { get;  set; } = string.Empty;
	public string PhoneNumber { get;  set; } = string.Empty;
	public DateOnly BirthDate { get;  set; }
	public DateTime CreatedAt { get;   set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }
}