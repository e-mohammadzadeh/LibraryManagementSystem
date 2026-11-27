using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
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
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthorManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly PersonManagementService _personService;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;
	private readonly IBookAuthorRepository _bookAuthorRepository;


	public AuthorManagementService(IAuthorRepository authorRepository, PersonManagementService personService,
		IAuthorizationService authorization, IAuditLogManagementService auditLog,
		IBookAuthorRepository bookAuthorRepository)
	{
		_authorRepository = authorRepository;
		_personService = personService;
		_authorization = authorization;
		_auditLog = auditLog;
		_bookAuthorRepository = bookAuthorRepository;
	}


	public ServiceResult<ContributorDto> AddAuthor(ContributorDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var dateValidation = Validator.ContributorDateValidator(dto.BirthDate, dto.DeathDate);
		if (!dateValidation.IsValid) return ServiceResult<ContributorDto>.Fail(dateValidation.ErrorMessage!);

		var personResult = _personService.ResolveOrCreatePerson(dto);
		if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message!);
		var person = personResult.Data!;

		var existingAuthor = _authorRepository.GetByPersonId(person.Id, EntityFilter.All);
		if (existingAuthor is not null)
		{
			if (!existingAuthor.IsRemoved)
				return ServiceResult<ContributorDto>.Fail("This person already has an active author role.");

			return ServiceResult<ContributorDto>.Fail(
				"This author role has been removed and requires admin restoration.");
		}

		var author = new Author
		{
			PersonId = person.Id,
			Person = person,
			DeathDate = dto.DeathDate,
			Biography = dto.Biography
		};

		_authorRepository.Add(author);
		_auditLog.Record(AuditAction.AuthorCreated, "Author", author.Id, "Author role created.");
		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorAddedSuccessfully);
	}


	public ServiceResult<ContributorDto> UpdateAuthor(ContributorDto dto, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var author = _authorRepository.GetById(dto.Id, EntityFilter.Active);
		if (author is null) return ServiceResult<ContributorDto>.Fail(Messages.AuthorUpdateFailed);

		var person = author.Person;
		var personChanged = person.FirstName != dto.FirstName ||
		                    person.LastName != dto.LastName ||
		                    person.NationalCode != dto.NationalCode ||
		                    person.Email != dto.Email ||
		                    person.PhoneNumber != dto.PhoneNumber ||
		                    person.BirthDate != dto.BirthDate;

		var authorChanged = author.DeathDate != dto.DeathDate ||
		                    author.Biography != dto.Biography;

		if (!personChanged && !authorChanged)
			return ServiceResult<ContributorDto>.Warning(author.ToDto(), Messages.NoChangesDetected);

		if (personChanged)
		{
			var personResult = _personService.UpdatePerson(person.Id, dto, updatedBy);
			if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message!);
		}

		if (authorChanged)
		{
			author.DeathDate = dto.DeathDate;
			author.Biography = dto.Biography;
			author.UpdatedByUserId = updatedBy;
			_authorRepository.Update(author);
		}

		_auditLog.Record(AuditAction.AuthorUpdated, "Author", author.Id, "Author and/or Person information updated.");
		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorUpdatedSuccessfully);
	}


	public ServiceResult<ContributorDto> RemoveAuthor(Guid id)
	{
		var author = _authorRepository.GetById(id, EntityFilter.Active);
		if (author is null) return ServiceResult<ContributorDto>.Fail(Messages.AuthorRemoveFailed);

		var hasBooks = _bookAuthorRepository.GetByAuthorId(id, EntityFilter.Active).Any();
		if (hasBooks) return ServiceResult<ContributorDto>.Fail(Messages.AuthorHasAssociatedBooks);

		_authorRepository.Remove(author);
		_auditLog.Record(AuditAction.AuthorRemoved, "Author", id, "Author role removed.");
		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, ContributorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermissions = field switch
		{
			ContributorSearchField.Name => new[] { Permission.SearchForMember, Permission.FullSearchContributor },
			ContributorSearchField.Email => new[] { Permission.SearchForMember, Permission.FullSearchContributor },
			ContributorSearchField.NationalCode => new[] { Permission.FullSearchContributor },
			ContributorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};
		if (!_authorization.HasAnyPermission(requiredPermissions)) return [];

		var authors = _authorRepository.GetAll(EntityFilter.Active);

		return
		[
			.. authors
				.Where(author =>
				{
					var value = field switch
					{
						ContributorSearchField.Name => $"{author.Person.FirstName} {author.Person.LastName}",
						ContributorSearchField.NationalCode => author.Person.NationalCode,
						ContributorSearchField.Email => author.Person.Email,
						ContributorSearchField.PhoneNumber => author.Person.PhoneNumber,
						_ => throw new ArgumentOutOfRangeException(nameof(field))
					};

					return value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(author => author.ToDto())
		];
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var authors = _authorRepository.GetAll(EntityFilter.Active).Select(author => author.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => dto => dto.FirstName,
			ContributorSortField.LastName => dto => dto.LastName,
			ContributorSortField.FullName => dto => $"{dto.FirstName} {dto.LastName}",
			ContributorSortField.NationalCode => dto => dto.NationalCode,
			ContributorSortField.Email => dto => dto.Email,
			ContributorSortField.BirthDate => dto => dto.BirthDate,
			ContributorSortField.BookCount => dto => GetBooksByAuthor(dto.Id).Count,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? authors.OrderBy(keySelector)
			: authors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		return _bookAuthorRepository
			.GetByAuthorId(authorId, EntityFilter.Active)
			.Select(bookAuthor => bookAuthor.Book.ToDto())
			.ToList();
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];

		return _authorRepository
			.GetAll(EntityFilter.Removed)
			.Select(author => author.ToDto())
			.ToList();
	}
}