namespace LibraryManagementSystem.Domain.Entities;

public abstract class Person
{
	protected Person(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate)
	{
		Id = Guid.CreateVersion7();
		FirstName = firstName;
		LastName = lastName;
		NationalCode = nationalCode;
		Email = email;
		PhoneNumber = phoneNumber;
		BirthDate = birthDate;
		CreatedAt = DateTime.UtcNow;
		IsRemoved = false;
	}


	public Guid Id { get;  set; }
	public string FirstName { get;  set; }
	public string LastName { get;  set; }
	public string NationalCode { get;  set; }
	public string Email { get;  set; }
	public string PhoneNumber { get;  set; }
	public DateOnly BirthDate { get;  set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }
}