using LibraryManagementSystem.Application.Validators;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;


namespace LibraryManagementSystem.Application.Services;

public class PersonManagementService
{
	private readonly IPersonRepository _personRepository;
	private readonly PersonUniquenessChecker _uniquenessChecker;


	public PersonManagementService(IPersonRepository personRepository, PersonUniquenessChecker uniquenessChecker)
	{
		_personRepository = personRepository;
		_uniquenessChecker = uniquenessChecker;
	}


	public ServiceResult<Person> ResolveOrCreatePerson(ContributorDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var existingPerson = _personRepository.GetByNationalCode(dto.NationalCode, EntityFilter.All);
		if (existingPerson is not null) return ServiceResult<Person>.Ok(existingPerson, "Existing Person was found.");
		
		var duplicate = _uniquenessChecker.Check(dto.NationalCode, dto.Email, dto.PhoneNumber);
		var duplicateMessage = GetDuplicateMessage(duplicate);
		if (duplicateMessage is not null) return ServiceResult<Person>.Fail(duplicateMessage);

		var person = new Person
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate
		};

		_personRepository.Add(person);
		return ServiceResult<Person>.Ok(person, "Person was created successfully.");
	}


	public ServiceResult<Person> UpdatePerson(Guid personId, ContributorDto dto, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var person = _personRepository.GetById(personId);
		if (person is null) return ServiceResult<Person>.Fail("Person was not found.");

		var duplicate = _uniquenessChecker.Check(person, dto.NationalCode, dto.Email, dto.PhoneNumber);
		var duplicateMessage = GetDuplicateMessage(duplicate);
		if (duplicateMessage is not null) return ServiceResult<Person>.Fail(duplicateMessage);

		var noChanges =
			person.FirstName == dto.FirstName &&
			person.LastName == dto.LastName &&
			person.NationalCode == dto.NationalCode &&
			person.Email == dto.Email &&
			person.PhoneNumber == dto.PhoneNumber &&
			person.BirthDate == dto.BirthDate;

		if (noChanges) return ServiceResult<Person>.Warning(person, Messages.NoChangesDetected);

		person.FirstName = dto.FirstName;
		person.LastName = dto.LastName;
		person.NationalCode = dto.NationalCode;
		person.Email = dto.Email;
		person.PhoneNumber = dto.PhoneNumber;
		person.BirthDate = dto.BirthDate;
		person.UpdatedByUserId = updatedBy;

		_personRepository.Update(person);
		return ServiceResult<Person>.Ok(person, "Person information was updated successfully.");
	}


	private static bool HasActiveRole(Person person)
	{
		return
			(person.Author is not null && !person.Author.IsRemoved) ||
			(person.Translator is not null && !person.Translator.IsRemoved) ||
			(person.User is not null && !person.User.IsRemoved);
	}


	private static string? GetDuplicateMessage(DuplicateField duplicate)
	{
		return duplicate switch
		{
			DuplicateField.NationalCode => Messages.DuplicateNationalCodeMessage,
			DuplicateField.Email => Messages.DuplicateEmailMessage,
			DuplicateField.PhoneNumber => Messages.DuplicatePhoneNumberMessage,
			DuplicateField.None => null,
			_ => throw new ArgumentOutOfRangeException(nameof(duplicate))
		};
	}
}