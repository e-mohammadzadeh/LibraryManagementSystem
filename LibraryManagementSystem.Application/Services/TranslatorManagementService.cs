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

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;
	private readonly PersonManagementService _personService;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;
	private readonly IBookTranslatorRepository _bookTranslatorRepository;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, PersonManagementService personService, IAuthorizationService authorization,
		IAuditLogManagementService auditLog, IBookTranslatorRepository bookTranslatorRepository)
	{
		_translatorRepository = translatorRepository;
		_personService = personService;
		_authorization = authorization;
		_auditLog = auditLog;
		_bookTranslatorRepository = bookTranslatorRepository;
	}


	public ServiceResult<ContributorDto> AddTranslator(ContributorDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var dateValidation = Validator.ContributorDateValidator(dto.BirthDate, dto.DeathDate);
		if (!dateValidation.IsValid) return ServiceResult<ContributorDto>.Fail(dateValidation.ErrorMessage!);

		var personResult = _personService.ResolveOrCreatePerson(dto);
		if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message!);
		var person = personResult.Data!;

		var existingTranslator = _translatorRepository.
		var result = AddContributor(
			dto,
			Messages.DuplicateNationalCodeMessage,
			Messages.DuplicateEmailMessage,
			Messages.DuplicatePhoneNumberMessage,
			Messages.TranslatorAddedSuccessfully);

		if (result.IsSuccess)
			_auditLog.Record(AuditAction.TranslatorCreated, "Translator", result.Data!.Id, "Translator created.");
		return result;
	}


	public ServiceResult<ContributorDto> UpdateTranslator(ContributorDto dto, Guid? updatedBy = null)
	{
		var result = UpdateContributor(
			dto,
			updatedBy,
			Messages.TranslatorUpdateFailed,
			Messages.DuplicateNationalCodeMessage,
			Messages.DuplicateEmailMessage,
			Messages.DuplicatePhoneNumberMessage,
			Messages.TranslatorUpdatedSuccessfully,
			out var auditDetails);

		if (result.IsSuccess)
			_auditLog.Record(AuditAction.TranslatorUpdated, "Translator", dto.Id,
				auditDetails ?? "Translator updated.");
		return result;
	}


	public ServiceResult<ContributorDto> RemoveTranslator(Guid id)
	{
		var result = RemoveContributor(
			id,
			translatorId => _bookTranslatorRepository.GetByTranslatorId(translatorId, EntityFilter.Active).Count > 0,
			Messages.TranslatorRemoveFailed,
			Messages.TranslatorHasAssociatedBooks,
			Messages.TranslatorRemovedSuccessfully);

		if (result.IsSuccess) _auditLog.Record(AuditAction.TranslatorRemoved, "Translator", id, "Translator removed.");
		return result;
	}


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, ContributorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		Func<Translator, string?> selector = field switch
		{
			ContributorSearchField.Name => t => $"{t.FirstName} {t.LastName}",
			ContributorSearchField.NationalCode => t => t.NationalCode,
			ContributorSearchField.Email => t => t.Email,
			ContributorSearchField.PhoneNumber => t => t.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return SearchContributor(searchItem, field, selector);
	}


	public IReadOnlyList<ContributorDto> GetAllTranslators(
		ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		return GetAllContributors(sortField, sortDirection, id => GetBooksByTranslator(id).Count);
	}


	public IReadOnlyList<BookDto> GetBooksByTranslator(Guid translatorId)
	{
		//return GetBooksByContributor(translatorId, t => t.BookTranslators.Select(bt => bt.Book.ToDto()));
		return GetBooksByContributor(translatorId,
			id => _bookTranslatorRepository.GetByTranslatorId(id, EntityFilter.Active).Select(bt => bt.Book.ToDto()));
	}


	public IReadOnlyList<ContributorDto> GetRemovedTranslators() => GetRemovedContributors();
}