namespace LibraryManagementSystem;

public abstract class Person
{
	protected Person(string firstName, string lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly? BirthDate { get; set; }
}