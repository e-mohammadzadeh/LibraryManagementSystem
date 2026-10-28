using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
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
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public AuthorManagementService(IAuthorRepository authorRepository, IAuthorizationService authorization,
		IAuditLogManagementService auditLog)
	{
		_authorRepository = authorRepository;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddAuthor(ContributorDto dto)
	{
		if (_authorRepository.Exists(dto.NationalCode, a => a.NationalCode))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		if (_authorRepository.Exists(dto.Email, a => a.Email))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);

		if (_authorRepository.Exists(dto.PhoneNumber, a => a.PhoneNumber))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);

		var newAuthor = new Author
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate,
			Biography = dto.Biography
		};

		_authorRepository.Add(newAuthor);
		_auditLog.Record(AuditAction.AuthorCreated, "Author", newAuthor.Id, "Author created.");

		return ServiceResult<ContributorDto>.Ok(newAuthor.ToDto(), Messages.AuthorAddedSuccessfully);
	}


	public ServiceResult<ContributorDto> UpdateAuthor(ContributorDto dto, Guid? updatedBy = null)
	{
		var author = _authorRepository.GetById(dto.Id, EntityFilter.Active);
		if (author is null || author.IsRemoved) return ServiceResult<ContributorDto>.Fail(Messages.AuthorUpdateFailed);

		if (IsNoOpUpdateAuthor(author, dto))
			return ServiceResult<ContributorDto>.Warning(author.ToDto(), Messages.NoChangesDetected);

		if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
		    string.IsNullOrWhiteSpace(dto.NationalCode) || string.IsNullOrWhiteSpace(dto.Email) ||
		    string.IsNullOrWhiteSpace(dto.PhoneNumber))
			return ServiceResult<ContributorDto>.Fail(Messages.AuthorUpdateFailed);

		if (_authorRepository.Exists(dto.NationalCode, a => a.NationalCode, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		if (_authorRepository.Exists(dto.Email, a => a.Email, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);

		if (_authorRepository.Exists(dto.PhoneNumber, a => a.PhoneNumber, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);

		var auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(author, dto);

		author.FirstName = dto.FirstName;
		author.LastName = dto.LastName;
		author.NationalCode = dto.NationalCode;
		author.Email = dto.Email;
		author.PhoneNumber = dto.PhoneNumber;
		author.BirthDate = dto.BirthDate;
		author.Biography = dto.Biography;
		author.UpdatedByUserId = updatedBy;

		_authorRepository.Update(author);
		_auditLog.Record(AuditAction.AuthorUpdated, "Author", dto.Id, auditDetails ?? "Author updated.");

		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateAuthor(Author author, ContributorDto dto)
	{
		return (dto.FirstName == author.FirstName) &&
		       (dto.LastName == author.LastName) &&
		       (dto.NationalCode == author.NationalCode) &&
		       (dto.Email == author.Email) &&
		       (dto.PhoneNumber == author.PhoneNumber) &&
		       (dto.BirthDate == author.BirthDate) &&
		       (dto.Biography == null || dto.Biography == author.Biography);
	}


	public ServiceResult<ContributorDto> RemoveAuthor(Guid authorId)
	{
		var author = _authorRepository.GetById(authorId, EntityFilter.Active);
		if (author is null || author.IsRemoved) return ServiceResult<ContributorDto>.Fail(Messages.AuthorRemoveFailed);

		if (author.BookAuthors.Count != 0) return ServiceResult<ContributorDto>.Fail(Messages.AuthorHasAssociatedBooks);

		_authorRepository.Remove(author);
		_auditLog.Record(AuditAction.AuthorRemoved, "Author", authorId, "Author removed.");

		return ServiceResult<ContributorDto>.Ok(author.ToDto(), Messages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, AuthorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermission = field switch
		{
			AuthorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			AuthorSearchField.NationalCode => [Permission.FullSearchContributor],
			AuthorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			AuthorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
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

		return
		[
			.. _authorRepository.GetAll(EntityFilter.Active)
				.Where(author =>
				{
					var value = selector(author);
					return value is not null &&
					       value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(author => author.ToDto())
		];
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		var author = _authorRepository.GetById(authorId, EntityFilter.Active);
		if (author is null) return [];
		return [.. author.BookAuthors.Select(b => b.Book.ToDto())];
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var authors = _authorRepository.GetAll(EntityFilter.Active).Select(a => a.ToDto());
		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => a => a.FirstName,
			ContributorSortField.LastName => a => a.LastName,
			ContributorSortField.FullName => a => $"{a.FirstName} {a.LastName}",
			ContributorSortField.NationalCode => a => a.NationalCode,
			ContributorSortField.Email => a => a.Email,
			ContributorSortField.BirthDate => a => a.BirthDate,
			ContributorSortField.BookCount => a => GetBooksByAuthor(a.Id).Count,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? authors.OrderBy(keySelector)
			: authors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];
		return [.. _authorRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}
}