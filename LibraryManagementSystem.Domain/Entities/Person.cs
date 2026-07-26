namespace LibraryManagementSystem.Domain.Entities;

public abstract class Person
{
	protected Person(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate)
	{
		FirstName = firstName;
		LastName = lastName;
		NationalCode = nationalCode;
		Email = email;
		PhoneNumber = phoneNumber;
		BirthDate = birthDate;
		CreatedAt = DateTime.Now;
	}


	public int Id { get; protected init; }
	public string FirstName { get; private set; }
	public string LastName { get; private set; }
	public string NationalCode { get; private set; }
	public string Email { get; private set; }
	public string PhoneNumber { get; private set; }
	public DateOnly BirthDate { get; private set; }
	public DateTime CreatedAt { get; protected set; }
	public DateTime? UpdatedAt { get; protected set; }


	protected void MarkAsUpdated() { UpdatedAt = DateTime.Now; }


	protected void UpdateCore(string? firstName, string? lastName, string? nationalCode, string? email,
		string? phoneNumber, DateOnly? birthDate)
	{
		MarkAsUpdated();
		FirstName = firstName ?? FirstName;
		LastName = lastName ?? LastName;
		NationalCode = nationalCode ?? NationalCode;
		Email = email ?? Email;
		PhoneNumber = phoneNumber ?? PhoneNumber;
		BirthDate = birthDate ?? BirthDate;
	}
}