using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
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
		var result = AddContributor(
			dto,
			Messages.DuplicateAuthorsNotAllowedByNationalCode,
			Messages.DuplicateAuthorsNotAllowedByEmail,
			Messages.DuplicateAuthorsNotAllowedByPhoneNumber,
			Messages.AuthorAddedSuccessfully);

		if (result.IsSuccess) _auditLog.Record(AuditAction.AuthorCreated, "Author", result.Data!.Id, "Author created.");
		return result;
	}


	public ServiceResult<ContributorDto> UpdateAuthor(ContributorDto dto, Guid? updatedBy = null)
	{
		var result = UpdateContributor(
			dto,
			updatedBy,
			Messages.AuthorUpdateFailed,
			Messages.DuplicateAuthorsNotAllowedByNationalCode,
			Messages.DuplicateAuthorsNotAllowedByEmail,
			Messages.DuplicateAuthorsNotAllowedByPhoneNumber,
			Messages.AuthorUpdatedSuccessfully,
			out var auditDetails);

		if (result.IsSuccess)
			_auditLog.Record(AuditAction.AuthorUpdated, "Author", dto.Id, auditDetails ?? "Author updated.");
		return result;
	}
	

	public ServiceResult<ContributorDto> RemoveAuthor(Guid id)
	{
		var result = RemoveContributor(
			id,
			a => a.BookAuthors.Count != 0,
			Messages.AuthorRemoveFailed,
			Messages.AuthorHasAssociatedBooks,
			Messages.AuthorRemovedSuccessfully);

		if (result.IsSuccess) _auditLog.Record(AuditAction.AuthorRemoved, "Author", id, "Author removed.");
		return result;
	}


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, ContributorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermission = field switch
		{
			ContributorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.NationalCode => [Permission.FullSearchContributor],
			ContributorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!_authorization.HasAnyPermission(requiredPermission)) return [];

		Func<Author, string?> selector = field switch
		{
			ContributorSearchField.Name => a => $"{a.FirstName} {a.LastName}",
			ContributorSearchField.NationalCode => a => a.NationalCode,
			ContributorSearchField.Email => a => a.Email,
			ContributorSearchField.PhoneNumber => a => a.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return SearchContributor(searchItem, selector);
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		return GetAllContributors(sortField, sortDirection, id => GetBooksByAuthor(id).Count);
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		return GetBooksByContributor(authorId, a => a.BookAuthors.Select(ba => ba.Book.ToDto()));
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors()
	{
		return !_authorization.HasPermission(Permission.ViewRemovedContributors) ? [] : GetRemovedContributors();
	}
}