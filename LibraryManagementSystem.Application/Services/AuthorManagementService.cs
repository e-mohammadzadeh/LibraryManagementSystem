using LibraryManagementSystem.Application.Authorization;
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

public class AuthorManagementService : ContributorManagementService<Author>
{
	private readonly IBookAuthorRepository _bookAuthorRepository;


	public AuthorManagementService(IAuthorRepository authorRepository, PersonManagementService personService,
		IAuthorizationService authorization, IAuditLogManagementService auditLog,
		IBookAuthorRepository bookAuthorRepository, IUnitOfWork unitOfWork)
		: base(authorRepository, personService, authorization, auditLog, new ContributorMessages(
				Messages.AuthorAddedSuccessfully, Messages.AuthorUpdateFailed, Messages.AuthorUpdatedSuccessfully,
				Messages.AuthorRemoveFailed, Messages.UserRestoredSuccessfully, Messages.UserRestoreFailed,
				Messages.AuthorHasAssociatedBooks, Messages.AuthorRemovedSuccessfully,
				"This person already has an active author role.",
				"This author role has been removed and requires admin restoration."),
			new ContributorAudit("Author", AuditAction.AuthorCreated, AuditAction.AuthorUpdated,
				AuditAction.AuthorRemoved, AuditAction.AuthorRestored), unitOfWork)
	{
		_bookAuthorRepository = bookAuthorRepository;
	}


	protected override IEnumerable<BookDto> GetBooks(Guid authorId) =>
		_bookAuthorRepository.GetByAuthorId(authorId, EntityFilter.Active).Select(ba => ba.Book.ToDto());


	public ServiceResult<ContributorDto> AddAuthor(ContributorDto dto) => AddContributor(dto);


	public ServiceResult<ContributorDto> UpdateAuthor(ContributorDto dto, Guid? updatedBy = null) =>
		UpdateContributor(dto, updatedBy);


	public ServiceResult<ContributorDto> RemoveAuthor(Guid id, Guid? removedBy = null) =>
		RemoveContributor(id, removedBy);


	public IReadOnlyList<ContributorDto> SearchAuthor(string searchItem, ContributorSearchField field) =>
		SearchContributors(searchItem, field);


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending) =>
		GetAllContributors(sortField, sortDirection);


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId) => GetBooksOf(authorId);
	public IReadOnlyList<ContributorDto> GetRemovedAuthors() => GetRemovedContributors();
}