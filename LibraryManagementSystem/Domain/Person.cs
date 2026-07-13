namespace LibraryManagementSystem.Domain;

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
	}


	public int Id { get; protected set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string NationalCode { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public DateOnly BirthDate { get; set; }
	// TODO	Soft Deletes & Auditing: In real apps, add *IsDeleted*, *CreatedAt*, *UpdatedAt* fields early.

	protected void UpdateCore(string? firstName, string? lastName, string? nationalCode, string? email,
		string? phoneNumber, DateOnly? birthDate)
	{
		FirstName = firstName ?? FirstName;
		LastName = lastName ?? LastName;
		NationalCode = nationalCode ?? NationalCode;
		Email = email ?? Email;
		PhoneNumber = phoneNumber ?? PhoneNumber;
		BirthDate = birthDate ?? BirthDate;
	}
}