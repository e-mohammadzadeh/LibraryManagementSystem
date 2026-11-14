using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;


public sealed class PersonUniquenessChecker
{
	private readonly IPersonRepository _persons;

	public PersonUniquenessChecker(IPersonRepository persons) => _persons = persons;


	public DuplicateField Check(string nationalCode, Email email, PhoneNumber phoneNumber, Guid? excludeId = null)
	{
		if (_persons.Exists(p => p.NationalCode == nationalCode, excludeId)) return DuplicateField.NationalCode;
		if (_persons.Exists(p => p.Email == email, excludeId)) return DuplicateField.Email;
		if (_persons.Exists(p => p.PhoneNumber == phoneNumber, excludeId)) return DuplicateField.PhoneNumber;
		return DuplicateField.None;
	}
}