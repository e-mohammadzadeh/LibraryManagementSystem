using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;


	public TranslatorManagementService(ITranslatorRepository translatorRepository)
	{
		_translatorRepository = translatorRepository;
	}


	public ServiceResult<Translator> AddTranslator(CreateTranslatorDto dto)
	{
		string? warningMessage = null;

		if (_translatorRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByNationalCode);

		if (_translatorRepository.ExistsByEmail(dto.Email))
			return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByEmail);

		var existingSameName = _translatorRepository.FindByName(dto.FirstName, dto.LastName);
		if (existingSameName is not null)
			warningMessage = $"A translator with the name already exists (ID: {existingSameName.Id}).";

		var newTranslator = new Translator(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate);

		_translatorRepository.Add(newTranslator);
		return warningMessage is not null
			? ServiceResult<Translator>.Warning(newTranslator, warningMessage)
			: ServiceResult<Translator>.Ok(newTranslator, ValidationMessages.TranslatorAddedSuccessfully);
	}


	public IReadOnlyList<Translator> GetAllTranslators()
	{
		return _translatorRepository.GetAll();
	}


	public Translator? FindTranslatorById(int id)
	{
		return _translatorRepository.FindById(id);
	}


	public ServiceResult<Translator> UpdateTranslator(int translatorId, UpdateTranslatorDto dto)
	{
		var translator = FindTranslatorById(translatorId);
		if (translator is null)
			return ServiceResult<Translator>.Fail(ValidationMessages.TranslatorUpdateFailed);

		if (IsNoOpUpdateTranslator(translator, dto))
			return ServiceResult<Translator>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? translator.FirstName;
		var resolvedLastName = dto.LastName ?? translator.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_translatorRepository.ExistsByName(resolvedFirstName, resolvedLastName, translatorId))
				return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByName);
		}

		if (dto.NationalCode is not null && _translatorRepository.ExistsByNationalCode(dto.NationalCode, translatorId))
			return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByNationalCode);

		if (dto.Email is not null && _translatorRepository.ExistsByEmail(dto.Email, translatorId))
			return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByEmail);

		if (dto.PhoneNumber is not null && _translatorRepository.ExistsByPhoneNumber(dto.PhoneNumber, translatorId))
			return ServiceResult<Translator>.Fail(ValidationMessages.FailureDuplicateTranslatorByPhoneNumber);

		translator.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate);

		return ServiceResult<Translator>.Ok(translator, ValidationMessages.TranslatorUpdatedSuccessfully);
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


	public ServiceResult<Translator> RemoveTranslator(int translatorId) {
		var translator = FindTranslatorById(translatorId);
		if (translator is null)
			return ServiceResult<Translator>.Fail(ValidationMessages.TranslatorRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting translator should check that none of books isn't borrowed
		if (translator.Books.Count != 0)
			return ServiceResult<Translator>.Fail("Failed to remove translator. The translator has associated books.");

		_translatorRepository.Remove(translator);
		return ServiceResult<Translator>.Ok(translator, ValidationMessages.TranslatorRemovedSuccessfully);
	}


	public IReadOnlyList<Translator> SearchTranslator(string searchItem, Func<Translator, string?> selector) {
		return _translatorRepository.Search(searchItem, selector);
	}
}