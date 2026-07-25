namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public record PersonFieldsResult(
	string FirstName,
	string LastName,
	string NationalCode,
	string Email,
	string PhoneNumber,
	DateOnly BirthDate);