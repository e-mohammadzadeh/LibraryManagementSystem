using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;


	public TranslatorManagementService(ITranslatorRepository translatorRepository)
	{
		_translatorRepository = translatorRepository;
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


	public IReadOnlyList<TranslatorDto> GetAllTranslators()
	{
		return [.. _translatorRepository.GetAll().Select(translator => translator.ToDto())];
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
}