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

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, IAuthorizationService authorization,
		IAuditLogManagementService auditLog)
	{
		_translatorRepository = translatorRepository;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddTranslator(ContributorDto dto)
	{
		if (_translatorRepository.Exists(dto.NationalCode, t => t.NationalCode))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (_translatorRepository.Exists(dto.Email, t => t.Email))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (_translatorRepository.Exists(dto.PhoneNumber, t => t.PhoneNumber))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		var newTranslator = new Translator
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate,
			Biography = dto.Biography
		};

		_translatorRepository.Add(newTranslator);
		_auditLog.Record(AuditAction.TranslatorCreated, "Translator", newTranslator.Id, "Translator created.");

		return ServiceResult<ContributorDto>.Ok(newTranslator.ToDto(), Messages.TranslatorAddedSuccessfully);
	}


	public ServiceResult<ContributorDto> UpdateTranslator(ContributorDto dto, Guid? updatedBy = null)
	{
		var translator = _translatorRepository.GetById(dto.Id, EntityFilter.Active);
		if (translator is null || translator.IsRemoved)
			return ServiceResult<ContributorDto>.Fail(Messages.TranslatorUpdateFailed);

		if (IsNoOpUpdateTranslator(translator, dto))
			return ServiceResult<ContributorDto>.Fail(Messages.NoChangesDetected);

		if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
		    string.IsNullOrWhiteSpace(dto.NationalCode) || string.IsNullOrWhiteSpace(dto.Email) ||
		    string.IsNullOrWhiteSpace(dto.PhoneNumber))
			return ServiceResult<ContributorDto>.Fail(Messages.TranslatorUpdateFailed);

		if (_translatorRepository.Exists(dto.NationalCode, t => t.NationalCode, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (_translatorRepository.Exists(dto.Email, t => t.Email, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (_translatorRepository.Exists(dto.PhoneNumber, t => t.PhoneNumber, dto.Id))
			return ServiceResult<ContributorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		var auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(translator, dto);

		translator.FirstName = dto.FirstName;
		translator.LastName = dto.LastName;
		translator.NationalCode = dto.NationalCode;
		translator.Email = dto.Email;
		translator.PhoneNumber = dto.PhoneNumber;
		translator.BirthDate = dto.BirthDate;
		translator.Biography = dto.Biography;
		translator.UpdatedByUserId = updatedBy;

		_translatorRepository.Update(translator);
		_auditLog.Record(AuditAction.TranslatorUpdated, "Translator", dto.Id, auditDetails ?? "Translator updated.");

		return ServiceResult<ContributorDto>.Ok(translator.ToDto(), Messages.TranslatorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateTranslator(Translator translator, ContributorDto dto)
	{
		return (dto.FirstName == translator.FirstName) &&
		       (dto.LastName == translator.LastName) &&
		       (dto.NationalCode == translator.NationalCode) &&
		       (dto.Email == translator.Email) &&
		       (dto.PhoneNumber == translator.PhoneNumber) &&
		       (dto.BirthDate == translator.BirthDate) &&
		       (dto.Biography == null || dto.Biography == translator.Biography);
	}


	public ServiceResult<ContributorDto> RemoveTranslator(Guid translatorId)
	{
		var translator = _translatorRepository.GetById(translatorId, EntityFilter.Active);
		if (translator is null || translator.IsRemoved)
			return ServiceResult<ContributorDto>.Fail(Messages.TranslatorRemoveFailed);

		if (translator.BookTranslators.Count != 0)
			return ServiceResult<ContributorDto>.Fail(Messages.TranslatorHasAssociatedBooks);

		_translatorRepository.Remove(translator);
		_auditLog.Record(AuditAction.TranslatorRemoved, "Translator", translatorId, "Translator removed.");

		return ServiceResult<ContributorDto>.Ok(translator.ToDto(), Messages.TranslatorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, TranslatorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermission = field switch
		{
			TranslatorSearchField.Name => [Permission.SearchTranslatorForMember, Permission.FullSearchTranslator],
			TranslatorSearchField.NationalCode => [Permission.FullSearchTranslator],
			TranslatorSearchField.Email => [Permission.SearchTranslatorForMember, Permission.FullSearchTranslator],
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

		return
		[
			.. _translatorRepository.GetAll(EntityFilter.Active)
				.Where(translator =>
				{
					var value = selector(translator);

					return value is not null &&
					       value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(translator => translator.ToDto())
		];
	}


	public IReadOnlyList<BookDto> GetBooksByTranslator(Guid translatorId)
	{
		var translator = _translatorRepository.GetById(translatorId, EntityFilter.Active);
		if (translator is null) return [];
		return [.. translator.BookTranslators.Select(b => b.Book.ToDto())];
	}


	public IReadOnlyList<ContributorDto> GetAllTranslators(ContributorSortField sortField = ContributorSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var translators = _translatorRepository.GetAll(EntityFilter.Active).Select(t => t.ToDto());
		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.Id => t => t.Id,
			ContributorSortField.FirstName => t => t.FirstName,
			ContributorSortField.LastName => t => t.LastName,
			ContributorSortField.FullName => t => $"{t.FirstName} {t.LastName}",
			ContributorSortField.NationalCode => t => t.NationalCode,
			ContributorSortField.Email => t => t.Email,
			ContributorSortField.BirthDate => t => t.BirthDate,
			ContributorSortField.BookCount => t => GetBooksByTranslator(t.Id).Count,
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
}