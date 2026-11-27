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

public class TranslatorManagementService : ContributorManagementService<Translator>
{
	private readonly IBookTranslatorRepository _bookTranslatorRepository;


	public TranslatorManagementService(ITranslatorRepository translatorRepository,
		PersonManagementService personService,
		IAuthorizationService authorization, IAuditLogManagementService auditLog,
		IBookTranslatorRepository bookTranslatorRepository, IUnitOfWork unitOfWork)
		: base(translatorRepository, personService, authorization, auditLog,
			new ContributorMessages(
				Messages.TranslatorAddedSuccessfully, Messages.TranslatorUpdateFailed,
				Messages.TranslatorUpdatedSuccessfully, Messages.TranslatorRemoveFailed,
				Messages.TranslatorRestoredSuccessfully, Messages.TranslatorRestoreFailed,
				Messages.TranslatorHasAssociatedBooks, Messages.TranslatorRemovedSuccessfully,
				"This person already has an active translator role.",
				"This translator role has been removed and requires admin restoration."),
			new ContributorAudit("Translator", AuditAction.TranslatorCreated, AuditAction.TranslatorUpdated,
				AuditAction.TranslatorRemoved, AuditAction.TranslatorRestored), unitOfWork)
	{
		_bookTranslatorRepository = bookTranslatorRepository;
	}


	protected override IEnumerable<BookDto> GetBooks(Guid translatorId) =>
		_bookTranslatorRepository.GetByTranslatorId(translatorId, EntityFilter.Active).Select(ba => ba.Book.ToDto());


	public ServiceResult<ContributorDto> AddTranslator(ContributorDto dto) => AddContributor(dto);


	public ServiceResult<ContributorDto> UpdateTranslator(ContributorDto dto, Guid? updatedBy = null) =>
		UpdateContributor(dto, updatedBy);


	public ServiceResult<ContributorDto> RemoveTranslator(Guid id, Guid? removedBy = null) =>
		RemoveContributor(id, removedBy);


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, ContributorSearchField field) =>
		SearchContributors(searchItem, field);


	public ServiceResult<ContributorDto> RestoreTranslator(Guid id, Guid? restoredBy = null) =>
		RestoreContributor(id, restoredBy);


	public IReadOnlyList<ContributorDto> GetAllTranslators(
		ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending) =>
		GetAllContributors(sortField, sortDirection);


	public IReadOnlyList<BookDto> GetBooksByTranslator(Guid translatorId) => GetBooksOf(translatorId);
	public IReadOnlyList<ContributorDto> GetRemovedTranslators() => GetRemovedContributors();
}