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

public class TranslatorManagementService : ContributorManagementService<Translator>
{
	private readonly IAuditLogManagementService _auditLog;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, IAuthorizationService authorization,
		IAuditLogManagementService auditLog) : base(translatorRepository, authorization)
	{
		_auditLog = auditLog;
	}


	public ServiceResult<ContributorDto> AddTranslator(ContributorDto dto)
	{
		var result = AddContributor(
			dto,
			Messages.DuplicateTranslatorsNotAllowedByNationalCode,
			Messages.DuplicateTranslatorsNotAllowedByEmail,
			Messages.DuplicateTranslatorsNotAllowedByPhoneNumber,
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
			Messages.DuplicateTranslatorsNotAllowedByNationalCode,
			Messages.DuplicateTranslatorsNotAllowedByEmail,
			Messages.DuplicateTranslatorsNotAllowedByPhoneNumber,
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
			t => t.BookTranslators.Count != 0,
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
		return GetBooksByContributor(translatorId, t => t.BookTranslators.Select(bt => bt.Book.ToDto()));
	}


	public IReadOnlyList<ContributorDto> GetRemovedTranslators() => GetRemovedContributors();
}