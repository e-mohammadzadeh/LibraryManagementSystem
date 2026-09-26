using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, IBookRepository bookRepository,
		IAuthorizationService authorization, IAuditLogManagementService auditLog)
	{
		_translatorRepository = translatorRepository;
		_bookRepository = bookRepository;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddTranslator(CreateContributorDto dto)
	{
		string? warningMessage = null;

		if (_translatorRepository.ExistsByNationalCode(dto.NationalCode, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (_translatorRepository.ExistsByEmail(dto.Email, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (_translatorRepository.ExistsByPhoneNumber(dto.PhoneNumber, null))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		var existingSameName = _translatorRepository.FindByName(dto.FirstName, dto.LastName);
		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateTranslatorNameWarning, existingSameName.Id);

		var newTranslator = new Translator(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_translatorRepository.Add(newTranslator);
		_auditLog.Record(AuditAction.TranslatorCreated, "Translator", newTranslator.Id, "Translator created.");

		return warningMessage is not null
			? ServiceResult<ContributorDto>.Warning(newTranslator.ToDto(), warningMessage)
			: ServiceResult<ContributorDto>.Ok(newTranslator.ToDto(), Messages.TranslatorAddedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> GetAllTranslators(TranslatorSortField sortField = TranslatorSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var translators = _translatorRepository.GetAll(EntityFilter.Active).Select(t => t.ToDto());
		Func<ContributorDto, object> keySelector = sortField switch
		{
			TranslatorSortField.Id => t => t.Id,
			TranslatorSortField.FirstName => t => t.FirstName,
			TranslatorSortField.LastName => t => t.LastName,
			TranslatorSortField.FullName => t => $"{t.FirstName} {t.LastName}",
			TranslatorSortField.NationalCode => t => t.NationalCode,
			TranslatorSortField.Email => t => t.Email,
			TranslatorSortField.BirthDate => t => t.BirthDate,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? translators.OrderBy(keySelector)
			: translators.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<ContributorDto> GetRemovedTranslators()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedTranslators)) return [];
		return [.. _translatorRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}



	private Translator? FindTranslatorById(Guid id) { return _translatorRepository.FindById(id); }


	public ServiceResult<ContributorDto> UpdateTranslator(Guid translatorId, UpdateContributorDto dto)
	{
		string? warningMessage = null;

		var translator = FindTranslatorById(translatorId);
		if (translator is null) return ServiceResult<ContributorDto>.Fail(Messages.TranslatorUpdateFailed);

		if (IsNoOpUpdateTranslator(translator, dto))
			return ServiceResult<ContributorDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? translator.FirstName;
		var resolvedLastName = dto.LastName ?? translator.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			var existingSameName = _translatorRepository.FindByName(resolvedFirstName, resolvedLastName);
			if (existingSameName is not null && existingSameName.Id != translatorId)
				warningMessage = string.Format(Messages.DuplicateTranslatorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode is not null && _translatorRepository.ExistsByNationalCode(dto.NationalCode, translatorId))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (dto.Email is not null && _translatorRepository.ExistsByEmail(dto.Email, translatorId))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (dto.PhoneNumber is not null && _translatorRepository.ExistsByPhoneNumber(dto.PhoneNumber, translatorId))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		var auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(translator, dto);

		_translatorRepository.Update(translator, dto);
		_auditLog.Record(AuditAction.TranslatorUpdated, "Translator", translatorId,
			auditDetails ?? "Translator updated.");

		return warningMessage is not null
			? ServiceResult<ContributorDto>.Warning(translator.ToDto(), warningMessage)
			: ServiceResult<ContributorDto>.Ok(translator.ToDto(), Messages.TranslatorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateTranslator(Translator translator, UpdateContributorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == translator.FirstName) &&
		       (dto.LastName == null || dto.LastName == translator.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == translator.NationalCode) &&
		       (dto.Email == null || dto.Email == translator.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == translator.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == translator.BirthDate);
	}


	public ServiceResult<ContributorDto> RemoveTranslator(Guid translatorId)
	{
		var translator = FindTranslatorById(translatorId);
		if (translator is null) return ServiceResult<ContributorDto>.Fail(Messages.TranslatorRemoveFailed);

		var booksByTranslator = _bookRepository.GetByTranslatorId(translatorId);
		if (booksByTranslator.Count != 0)
			return ServiceResult<ContributorDto>.Fail(Messages.TranslatorHasAssociatedBooks);

		_translatorRepository.Remove(translator);
		_auditLog.Record(AuditAction.TranslatorRemoved, "Translator", translatorId, "Translator removed.");

		return ServiceResult<ContributorDto>.Ok(translator.ToDto(), Messages.TranslatorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, TranslatorSearchField field)
	{
		var requiredPermission = field switch
		{
			TranslatorSearchField.Name => new[]
				{ Permission.SearchTranslatorForMember, Permission.FullSearchTranslator },
			TranslatorSearchField.NationalCode => new[] { Permission.FullSearchTranslator },
			TranslatorSearchField.Email => new[]
				{ Permission.SearchTranslatorForMember, Permission.FullSearchTranslator },
			TranslatorSearchField.PhoneNumber => new[] { Permission.FullSearchTranslator },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!_authorization.HasAnyPermission(requiredPermission)) return [];

		Func<Translator, string?> selector = field switch
		{
			TranslatorSearchField.Name => t => $"{t.FirstName} {t.LastName}",
			TranslatorSearchField.NationalCode => t => t.NationalCode,
			TranslatorSearchField.Email => t => t.Email,
			TranslatorSearchField.PhoneNumber => t => t.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _translatorRepository.Search(searchItem, selector).Select(translator => translator.ToDto())];
	}


	public IReadOnlyList<BookDto> GetBooksByTranslator(Guid translatorId)
	{
		var translator = _translatorRepository.FindById(translatorId);
		if (translator is null) return [];
		return [.. _bookRepository.GetByTranslatorId(translatorId).Select(b => b.ToDto())];
	}
}