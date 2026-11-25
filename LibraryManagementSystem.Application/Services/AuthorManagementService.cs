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
		IAuthorizationService authorization, IAuditLogManagementService auditLog, IBookAuthorRepository bookAuthorRepository)
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
				"This Author role has been removed and requires admin restoration.");
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
			if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message);
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

		var hasBooks = _bookAuthorRepository.GetByAuthorId(id, EntityFilter.Active).Count > 0;
		if (hasBooks) return ServiceResult<ContributorDto>.Fail(Messages.AuthorHasAssociatedBooks);

		_authorRepository.Remove(author);
		_auditLog.Record(AuditAction.AuthorRemoved, "Author", id, "Author role removed.");
		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, ContributorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		Func<Author, string?> selector = field switch
		{
			ContributorSearchField.Name => a => $"{a.FirstName} {a.LastName}",
			ContributorSearchField.NationalCode => a => a.NationalCode,
			ContributorSearchField.Email => a => a.Email,
			ContributorSearchField.PhoneNumber => a => a.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return SearchContributor(searchItem, field, selector);
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		return GetAllContributors(sortField, sortDirection, id => GetBooksByAuthor(id).Count);
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		//return GetBooksByContributor(authorId, a => a.BookAuthors.Select(ba => ba.Book.ToDto()));
		return GetBooksByContributor(authorId,
			id => _bookAuthorRepository.GetByAuthorId(id, EntityFilter.Active).Select(ba => ba.Book.ToDto()));
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors() => GetRemovedContributors();
}