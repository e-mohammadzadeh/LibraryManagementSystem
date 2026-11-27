using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Sort;

namespace LibraryManagementSystem.Application.Services;

public class TranslatorManagementService
{
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IAuthorizationService _authorization;
	private readonly IBookTranslatorRepository _bookTranslatorRepository;


	public TranslatorManagementService(ITranslatorRepository translatorRepository,
		IAuthorizationService authorization,
		 IBookTranslatorRepository bookTranslatorRepository)
	{
		_translatorRepository = translatorRepository;
		_authorization = authorization;
		_bookTranslatorRepository = bookTranslatorRepository;
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