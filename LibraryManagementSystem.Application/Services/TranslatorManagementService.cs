using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService : ContributorManagementService<Translator>
{
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public TranslatorManagementService(ITranslatorRepository translatorRepository, IAuthorizationService authorization,
		IAuditLogManagementService auditLog) : base(translatorRepository)
	{
		_authorization = authorization;
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


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, TranslatorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermission = field switch
		{
			TranslatorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			TranslatorSearchField.NationalCode => [Permission.FullSearchContributor],
			TranslatorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			TranslatorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
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


	public IReadOnlyList<ContributorDto> GetAllTranslators(
		ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var translators = _translatorRepository.GetAll(EntityFilter.Active).Select(t => t.ToDto());
		Func<ContributorDto, object> keySelector = sortField switch
		{
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
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];
		return [.. _translatorRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}
}