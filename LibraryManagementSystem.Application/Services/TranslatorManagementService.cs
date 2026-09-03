using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Enums.Search;
using LibraryManagementSystem.Domain.Enums.Sort;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IAuthorizationService _authorization;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, IAuthorizationService authorization)
	{
		_translatorRepository = translatorRepository;
		_authorization = authorization;
	}


	public ServiceResult<TranslatorDto> AddTranslator(CreateTranslatorDto dto)
	{
		string? warningMessage = null;

		if (_translatorRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (_translatorRepository.ExistsByEmail(dto.Email))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (_translatorRepository.ExistsByPhoneNumber(dto.PhoneNumber))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		var existingSameName = _translatorRepository.FindByName(dto.FirstName, dto.LastName);
		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateTranslatorNameWarning, existingSameName.Id);

		var newTranslator = new Translator(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate);

		_translatorRepository.Add(newTranslator);
		return warningMessage is not null
			? ServiceResult<TranslatorDto>.Warning(newTranslator.ToDto(), warningMessage)
			: ServiceResult<TranslatorDto>.Ok(newTranslator.ToDto(), Messages.TranslatorAddedSuccessfully);
	}


	public IReadOnlyList<TranslatorDto> GetAllTranslators(TranslatorSortField sortField = TranslatorSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var translators = _translatorRepository.GetAll(EntityFilter.Active).Select(t => t.ToDto());
		Func<TranslatorDto, object> keySelector = sortField switch
		{
			TranslatorSortField.Id => t => t.Id,
			TranslatorSortField.FirstName => t => t.FirstName,
			TranslatorSortField.LastName => t => t.LastName,
			TranslatorSortField.FullName => t => t.FullName,
			TranslatorSortField.NationalCode => t => t.NationalCode,
			TranslatorSortField.Email => t => t.Email,
			TranslatorSortField.BirthDate => t => t.BirthDate,
			TranslatorSortField.BookCount => t => t.BookCount,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? translators.OrderBy(keySelector)
			: translators.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<TranslatorDto> GetRemovedTranslators()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedTranslators)) return [];
		return [.. _translatorRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}



	private Translator? FindTranslatorById(int id) { return _translatorRepository.FindById(id); }


	public ServiceResult<TranslatorDto> UpdateTranslator(int translatorId, UpdateTranslatorDto dto)
	{
		string? warningMessage = null;

		var translator = FindTranslatorById(translatorId);
		if (translator is null) return ServiceResult<TranslatorDto>.Fail(Messages.TranslatorUpdateFailed);

		if (IsNoOpUpdateTranslator(translator, dto))
			return ServiceResult<TranslatorDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? translator.FirstName;
		var resolvedLastName = dto.LastName ?? translator.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			var existingSameName = _translatorRepository.FindByName(resolvedFirstName, resolvedLastName);
			if (existingSameName is not null && existingSameName.Id != translatorId)
				warningMessage = string.Format(Messages.DuplicateTranslatorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode is not null && _translatorRepository.ExistsByNationalCode(dto.NationalCode, translatorId))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByNationalCode);

		if (dto.Email is not null && _translatorRepository.ExistsByEmail(dto.Email, translatorId))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByEmail);

		if (dto.PhoneNumber is not null && _translatorRepository.ExistsByPhoneNumber(dto.PhoneNumber, translatorId))
			return ServiceResult<TranslatorDto>.Fail(Messages.DuplicateTranslatorsNotAllowedByPhoneNumber);

		translator.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate);

		_translatorRepository.Update(translator);
		return warningMessage is not null
			? ServiceResult<TranslatorDto>.Warning(translator.ToDto(), warningMessage)
			: ServiceResult<TranslatorDto>.Ok(translator.ToDto(), Messages.TranslatorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateTranslator(Translator translator, UpdateTranslatorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == translator.FirstName) &&
		       (dto.LastName == null || dto.LastName == translator.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == translator.NationalCode) &&
		       (dto.Email == null || dto.Email == translator.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == translator.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == translator.BirthDate);
	}


	public ServiceResult<TranslatorDto> RemoveTranslator(int translatorId)
	{
		var translator = FindTranslatorById(translatorId);
		if (translator is null) return ServiceResult<TranslatorDto>.Fail(Messages.TranslatorRemoveFailed);

		if (translator.BookTranslators.Count != 0)
			return ServiceResult<TranslatorDto>.Fail(Messages.TranslatorHasAssociatedBooks);

		_translatorRepository.Remove(translator);
		return ServiceResult<TranslatorDto>.Ok(translator.ToDto(), Messages.TranslatorRemovedSuccessfully);
	}


	public IReadOnlyList<TranslatorDto> SearchTranslator(string searchItem, TranslatorSearchField field)
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


	public IReadOnlyList<BookDto> GetBooksByTranslator(int translatorId)
	{
		var translator = _translatorRepository.FindById(translatorId);
		if (translator is null) return [];
		return [.. translator.BookTranslators.Select(bt => bt.Book.ToDto())];
	}
}