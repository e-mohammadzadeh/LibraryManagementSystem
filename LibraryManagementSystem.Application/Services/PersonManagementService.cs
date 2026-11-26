using LibraryManagementSystem.Application.Validators;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;

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
		if (existingPerson is not null)
		{
			if (!HasActiveRole(existingPerson))
				return ServiceResult<Person>.Fail("This Person has been removed. Admin restoration is required.");

			return ServiceResult<Person>.Ok(existingPerson, "Existing Person was found.");
		}

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

		var duplicate = PersonUniquenessChecker.Check(person, dto.NationalCode, dto.Email, dto.PhoneNumber);
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


	protected ServiceResult<ContributorDto> RemoveContributor(Guid id, Func<Guid, bool> hasAssociatedBooks,
		string removeFailedMessage, string hasAssociatedBooksMessage, string successMessage)
	{
		var contributor = Repository.GetById(id, EntityFilter.Active);
		if (contributor is null || contributor.IsRemoved)
			return ServiceResult<ContributorDto>.Fail(removeFailedMessage);

		if (hasAssociatedBooks(id)) return ServiceResult<ContributorDto>.Fail(hasAssociatedBooksMessage);

		Repository.Remove(contributor);
		return ServiceResult<ContributorDto>.Ok(contributor.ToDto(), successMessage);
	}


	protected IReadOnlyList<ContributorDto> SearchContributor(string searchItem, ContributorSearchField field,
		Func<TEntity, string?> selector)
	{
		var requiredPermission = field switch
		{
			ContributorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.NationalCode => [Permission.FullSearchContributor],
			ContributorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!Authorization.HasAnyPermission(requiredPermission)) return [];

		return
		[
			..
			Repository.GetAll(EntityFilter.Active)
				.Where(contributor =>
				{
					var value = selector(contributor);
					return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(contributor => contributor.ToDto())
		];
	}


	protected IReadOnlyList<ContributorDto> GetAllContributors(ContributorSortField sortField,
		SortDirection sortDirection, Func<Guid, int> getBookCount)
	{
		var contributors = Repository.GetAll(EntityFilter.Active).Select(c => c.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => c => c.FirstName,
			ContributorSortField.LastName => c => c.LastName,
			ContributorSortField.FullName => c => $"{c.FirstName} {c.LastName}",
			ContributorSortField.NationalCode => c => c.NationalCode,
			ContributorSortField.Email => c => c.Email.Value,
			ContributorSortField.BirthDate => c => c.BirthDate,
			ContributorSortField.BookCount => c => getBookCount(c.Id),
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? contributors.OrderBy(keySelector)
			: contributors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	protected IReadOnlyList<BookDto> GetBooksByContributor(Guid contributorId,
		Func<Guid, IEnumerable<BookDto>> bookSelector)
	{
		if (Repository.GetById(contributorId, EntityFilter.Active) is null) return [];
		return [.. bookSelector(contributorId)];
	}


	protected IReadOnlyList<ContributorDto> GetRemovedContributors()
	{
		if (!Authorization.HasPermission(Permission.ViewRemovedContributors)) return [];
		return [.. Repository.GetAll(EntityFilter.Removed).Select(contributor => contributor.ToDto())];
	}


	private static bool IsNoOpUpdate(TEntity entity, ContributorDto dto) =>
		dto.FirstName == entity.FirstName &&
		dto.LastName == entity.LastName &&
		dto.NationalCode == entity.NationalCode &&
		dto.Email == entity.Email &&
		dto.PhoneNumber == entity.PhoneNumber &&
		dto.BirthDate == entity.BirthDate &&
		dto.DeathDate == entity.DeathDate &&
		(dto.Biography is null || dto.Biography == entity.Biography);


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