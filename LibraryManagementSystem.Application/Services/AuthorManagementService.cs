using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Services;

public class AuthorManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public AuthorManagementService(IAuthorRepository authorRepository, IBookRepository bookRepository,
		IAuthorizationService authorization, IAuditLogManagementService auditLog)
	{
		_authorRepository = authorRepository;
		_bookRepository = bookRepository;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddAuthor(CreateContributorDto dto)
	{
		string? warningMessage = null;

		if (_authorRepository.ExistsByNationalCode(dto.NationalCode, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		var email = Email.Create(dto.Email);
		if (_authorRepository.ExistsByEmail(email, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);

		var phoneNumber = PhoneNumber.Create(dto.PhoneNumber);
		if (_authorRepository.ExistsByPhoneNumber(phoneNumber, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);

		var existingSameName = _authorRepository.FindByName(dto.FirstName, dto.LastName, EntityFilter.Active);

		if (existingSameName != null)
			warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);

		var newAuthor = new Author
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = dto.BirthDate,
			Biography = dto.Biography
		};

		_authorRepository.Add(newAuthor);
		_auditLog.Record(AuditAction.AuthorCreated, "Author", newAuthor.Id, "Author created.");

		return warningMessage != null
			? ServiceResult<ContributorDto>.Warning(newAuthor.ToDto(), warningMessage)
			: ServiceResult<ContributorDto>.Ok(newAuthor.ToDto(), Messages.AuthorAddedSuccessfully);
	}


	public ServiceResult<ContributorDto> UpdateAuthor(UpdateContributorDto dto, Guid? updateBy = null)
	{
		string? warningMessage = null;

		var author = _authorRepository.FindById(dto.Id, EntityFilter.Active);
		if (author is null) return ServiceResult<ContributorDto>.Fail(Messages.AuthorUpdateFailed);

		if (IsNoOpUpdateAuthor(author, dto)) return ServiceResult<ContributorDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName != null || dto.LastName != null)
		{
			var existingSameName = _authorRepository.FindByName(resolvedFirstName, resolvedLastName, EntityFilter.Active);
			if (existingSameName != null && existingSameName.Id != dto.Id)
				warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode != null && _authorRepository.ExistsByNationalCode(dto.NationalCode, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		if (dto.Email != null)
		{
			var email = Email.Create(dto.Email);
			if (_authorRepository.ExistsByEmail(email, dto.Id))
				return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);
		}

		if (dto.PhoneNumber != null)
		{
			var phoneNumber = PhoneNumber.Create(dto.PhoneNumber);
			if (_authorRepository.ExistsByPhoneNumber(phoneNumber, dto.Id))
				return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);
		}

		var auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(author, dto);

		_authorRepository.Update(author, updateBy);
		_auditLog.Record(AuditAction.AuthorUpdated, "Author", dto.Id, auditDetails ?? "Author updated.");

		return warningMessage != null
			? ServiceResult<ContributorDto>.Warning(author.ToDto(), warningMessage)
			: ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateAuthor(Author author, UpdateContributorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == author.FirstName) &&
		       (dto.LastName == null || dto.LastName == author.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == author.NationalCode) &&
		       (dto.Email == null || dto.Email == author.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == author.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == author.BirthDate) &&
		       (dto.Biography == null || dto.Biography == author.Biography);
	}


	public ServiceResult<ContributorDto> RemoveAuthor(Guid authorId)
	{
		var author = _authorRepository.FindById(authorId, EntityFilter.Active);
		if (author is null) return ServiceResult<ContributorDto>.Fail(Messages.AuthorRemoveFailed);

		var booksByAuthor = _bookRepository.GetByAuthorId(authorId, EntityFilter.Active);
		if (booksByAuthor.Count != 0) return ServiceResult<ContributorDto>.Fail(Messages.AuthorHasAssociatedBooks);

		_authorRepository.Remove(author);
		_auditLog.Record(AuditAction.AuthorRemoved, "Author", authorId, "Author removed.");

		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, AuthorSearchField field)
	{
		var requiredPermission = field switch
		{
			AuthorSearchField.Name => [Permission.SearchAuthorForMember, Permission.FullSearchAuthor],
			AuthorSearchField.NationalCode => [Permission.FullSearchAuthor],
			AuthorSearchField.Email => [Permission.SearchAuthorForMember, Permission.FullSearchAuthor],
			AuthorSearchField.PhoneNumber => new[] { Permission.FullSearchAuthor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!_authorization.HasAnyPermission(requiredPermission)) return [];

		Func<Author, string?> selector = field switch
		{
			AuthorSearchField.Name => a => $"{a.FirstName} {a.LastName}",
			AuthorSearchField.NationalCode => a => a.NationalCode,
			AuthorSearchField.Email => a => a.Email,
			AuthorSearchField.PhoneNumber => a => a.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _authorRepository.Search(searchItem, selector).Select(author => author.ToDto())];
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		var author = _authorRepository.FindById(authorId, EntityFilter.Active);
		if (author is null) return [];
		return [.. _bookRepository.GetByAuthorId(authorId, EntityFilter.Active).Select(b => b.ToDto())];
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(AuthorSortField sortField = AuthorSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		if (!_authorization.HasPermission(Permission.ViewAllUsers)) return [];

		var authors = _authorRepository.GetAll(EntityFilter.Active).Select(a => a.ToDto());
		Func<ContributorDto, object> keySelector = sortField switch
		{
			AuthorSortField.Id => a => a.Id,
			AuthorSortField.FirstName => a => a.FirstName,
			AuthorSortField.LastName => a => a.LastName,
			AuthorSortField.FullName => a => $"{a.FirstName} {a.LastName}",
			AuthorSortField.NationalCode => a => a.NationalCode,
			AuthorSortField.Email => a => a.Email,
			AuthorSortField.BirthDate => a => a.BirthDate,
			AuthorSortField.BookCount => a => _bookRepository.GetByAuthorId(a.Id, EntityFilter.Active).Count,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? authors.OrderBy(keySelector)
			: authors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedAuthors)) return [];
		return [.. _authorRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}
}