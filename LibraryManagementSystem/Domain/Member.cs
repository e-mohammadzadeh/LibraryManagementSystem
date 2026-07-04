namespace LibraryManagementSystem.Domain;

public class Member : LibraryUser
{
	public Member(string firstName, string lastName, string nationalCode, string email, string phoneNumber,
		DateOnly birthDate) : base(firstName, lastName, nationalCode, email, phoneNumber, birthDate)
	{
		Id = ++_nextMemberId;
	}


	private static int _nextMemberId;


	public void Update(string? firstName, string? lastName, string? nationalCode, string? email, string? phoneNumber, DateOnly? birthDate)
	{
		FirstName = firstName ?? FirstName;
		LastName = lastName ?? LastName;
		NationalCode = nationalCode ?? NationalCode;
		Email = email ?? Email;
		PhoneNumber = phoneNumber ?? PhoneNumber;
		BirthDate = birthDate ?? BirthDate;
	}
}