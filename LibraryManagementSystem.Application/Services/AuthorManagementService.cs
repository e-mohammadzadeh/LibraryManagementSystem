using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
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

public class AuthorManagementService : ContributorManagementService<Author>
{
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public AuthorManagementService(IAuthorRepository authorRepository, IAuthorizationService authorization,
		IAuditLogManagementService auditLog) : base(authorRepository)
	{
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddAuthor(ContributorDto dto)
	{
		var result = new AddContributor(
			dto,
			Messages.DuplicateAuthorsNotAllowedByNationalCode,
			Messages.DuplicateAuthorsNotAllowedByEmail,
			Messages.DuplicateAuthorsNotAllowedByPhoneNumber,
			Messages.AuthorAddedSuccessfully);

		if (result.IsSuccess)
			_auditLog.Record(AuditAction.AuthorCreated, "Author", result.Id, "Author created.");
		return ServiceResult<ContributorDto>.Ok(result.ToDto(), Messages.AuthorAddedSuccessfully);
	}


	public ServiceResult<ContributorDto> UpdateAuthor(ContributorDto dto, Guid? updatedBy = null)
	{
		var result = new UpdateContributor(
			dto,
			updatedBy,
			Messages.AuthorUpdateFailed,
			Messages.DuplicateAuthorsNotAllowedByNationalCode,
			Messages.DuplicateAuthorsNotAllowedByEmail,
			Messages.DuplicateAuthorsNotAllowedByPhoneNumber,
			Messages.AuthorUpdatedSuccessfully);

		if (result.IsSuccess)
		{
			var auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(author, dto);
			_auditLog.Record(AuditAction.AuthorUpdated, "Author", dto.Id, auditDetails ?? "Author updated.");
		}

		return result;
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