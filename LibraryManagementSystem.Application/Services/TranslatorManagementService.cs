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


	public TranslatorManagementService(ITranslatorRepository translatorRepository,
		PersonManagementService personService, IAuthorizationService authorization,
		IAuditLogManagementService auditLog, IBookTranslatorRepository bookTranslatorRepository)
	{
		_translatorRepository = translatorRepository;
		_personService = personService;
		_authorization = authorization;
		_auditLog = auditLog;
		_bookTranslatorRepository = bookTranslatorRepository;
	}


	public ServiceResult<ContributorDto> RemoveTranslator(Guid id)
	{
		var translator = _translatorRepository.GetById(id, EntityFilter.Active);
		if (translator is null) return ServiceResult<ContributorDto>.Fail(Messages.TranslatorRemoveFailed);

		var hasBooks = _bookTranslatorRepository.GetByTranslatorId(id, EntityFilter.Active).Count > 0;
		if (hasBooks) return ServiceResult<ContributorDto>.Fail(Messages.TranslatorHasAssociatedBooks);

		_translatorRepository.Remove(translator);
		_auditLog.Record(AuditAction.TranslatorRemoved, "Translator", id, "Translator role removed.");
		return ServiceResult<ContributorDto>.Ok(translator.ToDto(), Messages.TranslatorRemovedSuccessfully);
	}


	public IReadOnlyList<ContributorDto> SearchTranslator(string searchItem, ContributorSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		var requiredPermissions = field switch
		{
			ContributorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.NationalCode => [Permission.FullSearchContributor],
			ContributorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};
		if (!_authorization.HasAnyPermission(requiredPermissions)) return [];

		var translators = _translatorRepository.GetAll(EntityFilter.Active);

		return
		[
			.. translators
				.Where(translator =>
				{
					var value = field switch
					{
						ContributorSearchField.Name => $"{translator.Person.FirstName} {translator.Person.LastName}",
						ContributorSearchField.NationalCode => translator.Person.NationalCode,
						ContributorSearchField.Email => translator.Person.Email,
						ContributorSearchField.PhoneNumber => translator.Person.PhoneNumber,
						_ => throw new ArgumentOutOfRangeException(nameof(field))
					};

					return value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(translator => translator.ToDto())
		];
	}


	public IReadOnlyList<ContributorDto> GetAllTranslators(
		ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var translators = _translatorRepository.GetAll(EntityFilter.Active).Select(translator => translator.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => dto => dto.FirstName,
			ContributorSortField.LastName => dto => dto.LastName,
			ContributorSortField.FullName => dto => $"{dto.FirstName} {dto.LastName}",
			ContributorSortField.NationalCode => dto => dto.NationalCode,
			ContributorSortField.Email => dto => dto.Email,
			ContributorSortField.BirthDate => dto => dto.BirthDate,
			ContributorSortField.BookCount => dto => GetBooksByTranslator(dto.Id).Count,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? translators.OrderBy(keySelector)
			: translators.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<BookDto> GetBooksByTranslator(Guid translatorId)
	{
		return
		[
			.._bookTranslatorRepository
				.GetByTranslatorId(translatorId, EntityFilter.Active)
				.Select(bookAuthor => bookAuthor.Book.ToDto())
		];
	}


	public IReadOnlyList<ContributorDto> GetRemovedTranslators()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];

		return
		[
			.._translatorRepository
				.GetAll(EntityFilter.Removed)
				.Select(translator => translator.ToDto())
		];
	}
}