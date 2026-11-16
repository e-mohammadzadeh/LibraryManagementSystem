using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;


namespace LibraryManagementSystem.Application.Validators;

public sealed class PersonUniquenessChecker
{
	private readonly IPersonRepository _persons;

	public PersonUniquenessChecker(IPersonRepository persons) => _persons = persons;


	public DuplicateField Check(string nationalCode, Email email, PhoneNumber phoneNumber)
	{
		if (_persons.Exists(p => p.NationalCode == nationalCode))
			return DuplicateField.NationalCode;
		if (_persons.Exists(p => p.Email == email))
			return DuplicateField.Email;
		if (_persons.Exists(p => p.PhoneNumber == phoneNumber))
			return DuplicateField.PhoneNumber;
		return DuplicateField.None;
	}


	public DuplicateField Check(Person person, string nationalCode, Email email, PhoneNumber phoneNumber)
	{
		ArgumentNullException.ThrowIfNull(person);

		if (person.NationalCode != nationalCode && _persons.Exists(p => p.NationalCode == nationalCode, person.Id))
			return DuplicateField.NationalCode;
		if (!Equals(person.Email, email) && _persons.Exists(p => p.Email == email, person.Id))
			return DuplicateField.Email;
		if (!Equals(person.PhoneNumber, phoneNumber) && _persons.Exists(p => p.PhoneNumber == phoneNumber, person.Id))
			return DuplicateField.PhoneNumber;
		return DuplicateField.None;
	}
}