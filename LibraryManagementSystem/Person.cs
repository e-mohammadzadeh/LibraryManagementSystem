namespace LibraryManagementSystem;

public class Person
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public string? NationalCode { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public DateOnly BirthDate { get; set; }
	
}