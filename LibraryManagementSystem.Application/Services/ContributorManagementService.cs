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

namespace LibraryManagementSystem.Application.Services;

public abstract class ContributorManagementService<TEntity> where TEntity : Contributor, new()
{
	protected readonly IContributorRepository<TEntity> Repository;
	protected readonly IAuthorizationService Authorization;


	protected ContributorManagementService(IContributorRepository<TEntity> repository,
		IAuthorizationService authorization)
	{
		Repository = repository;
		Authorization = authorization;
	}


	protected ServiceResult<ContributorDto> AddContributor(ContributorDto dto, string duplicateNationalCodeMessage,
		string duplicateEmailMessage, string duplicatePhoneNumberMessage, string successMessage)
	{
		if (Repository.Exists(c => c.NationalCode == dto.NationalCode))
			return ServiceResult<ContributorDto>.Fail(duplicateNationalCodeMessage);

		if (Repository.Exists(c => c.Email == dto.Email))
			return ServiceResult<ContributorDto>.Fail(duplicateEmailMessage);

		if (Repository.Exists(c => c.PhoneNumber == dto.PhoneNumber))
			return ServiceResult<ContributorDto>.Fail(duplicatePhoneNumberMessage);

		var contributor = new TEntity
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate,
			Biography = dto.Biography
		};

		Repository.Add(contributor);
		return ServiceResult<ContributorDto>.Ok(contributor.ToDto(), successMessage);
	}


	protected ServiceResult<ContributorDto> UpdateContributor(ContributorDto dto, Guid? updatedBy,
		string notFoundMessage, string duplicateNationalCodeMessage, string duplicateEmailMessage,
		string duplicatePhoneNumberMessage, string successMessage, out string? auditDetails)
	{
		auditDetails = null;
		var contributor = Repository.GetById(dto.Id, EntityFilter.Active);
		if (contributor is null || contributor.IsRemoved) return ServiceResult<ContributorDto>.Fail(notFoundMessage);

		if (IsNoOpUpdate(contributor, dto))
			return ServiceResult<ContributorDto>.Warning(contributor.ToDto(), Messages.NoChangesDetected);

		if (Repository.Exists(c => c.NationalCode == dto.NationalCode, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicateNationalCodeMessage);

		if (Repository.Exists(c => c.Email == dto.Email, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicateEmailMessage);

		if (Repository.Exists(c => c.PhoneNumber == dto.PhoneNumber, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicatePhoneNumberMessage);

		auditDetails = PersonUpdateAuditDetailsBuilder.BuildPersonUpdateAuditDetails(contributor, dto);

		contributor.FirstName = dto.FirstName;
		contributor.LastName = dto.LastName;
		contributor.NationalCode = dto.NationalCode;
		contributor.Email = dto.Email;
		contributor.PhoneNumber = dto.PhoneNumber;
		contributor.BirthDate = dto.BirthDate;
		contributor.Biography = dto.Biography;
		contributor.UpdatedByUserId = updatedBy;

		Repository.Update(contributor);
		return ServiceResult<ContributorDto>.Ok(contributor.ToDto(), successMessage);
	}


	protected ServiceResult<ContributorDto> RemoveContributor(Guid id, Func<TEntity, bool> hasAssociatedBooks,
		string removeFailedMessage, string hasAssociatedBooksMessage, string successMessage)
	{
		var contributor = Repository.GetById(id, EntityFilter.Active);
		if (contributor is null || contributor.IsRemoved)
			return ServiceResult<ContributorDto>.Fail(removeFailedMessage);

		if (hasAssociatedBooks(contributor)) return ServiceResult<ContributorDto>.Fail(hasAssociatedBooksMessage);

		Repository.Remove(contributor);
		return ServiceResult<ContributorDto>.Ok(contributor.ToDto(), successMessage);
	}


	protected IReadOnlyList<ContributorDto> SearchContributor(string searchItem, ContributorSearchField field,
		Func<TEntity, string?> selector)
	{
		var requiredPermission = field switch
		{
			ContributorSearchField.Name => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.NationalCode => [Permission.FullSearchContributor],
			ContributorSearchField.Email => [Permission.SearchForMember, Permission.FullSearchContributor],
			ContributorSearchField.PhoneNumber => new[] { Permission.FullSearchContributor },
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!Authorization.HasAnyPermission(requiredPermission)) return [];

		return
		[
			..
			Repository.GetAll(EntityFilter.Active)
				.Where(contributor =>
				{
					var value = selector(contributor);
					return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(contributor => contributor.ToDto())
		];
	}


	protected IReadOnlyList<ContributorDto> GetAllContributors(ContributorSortField sortField,
		SortDirection sortDirection, Func<Guid, int> getBookCount)
	{
		var contributors = Repository.GetAll(EntityFilter.Active).Select(c => c.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => c => c.FirstName,
			ContributorSortField.LastName => c => c.LastName,
			ContributorSortField.FullName => c => $"{c.FirstName} {c.LastName}",
			ContributorSortField.NationalCode => c => c.NationalCode,
			ContributorSortField.Email => c => c.Email,
			ContributorSortField.BirthDate => c => c.BirthDate,
			ContributorSortField.BookCount => c => getBookCount(c.Id),
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? contributors.OrderBy(keySelector)
			: contributors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	protected IReadOnlyList<BookDto> GetBooksByContributor(Guid contributorId,
		Func<TEntity, IEnumerable<BookDto>> bookSelector)
	{
		var contributor = Repository.GetById(contributorId, EntityFilter.Active);
		if (contributor is null) return [];
		return [.. bookSelector(contributor)];
	}


	protected IReadOnlyList<ContributorDto> GetRemovedContributors()
	{
		if (!Authorization.HasPermission(Permission.ViewRemovedContributors)) return [];
		return [.. Repository.GetAll(EntityFilter.Removed).Select(contributor => contributor.ToDto())];
	}


	private static bool IsNoOpUpdate(TEntity entity, ContributorDto dto) =>
		dto.FirstName == entity.FirstName &&
		dto.LastName == entity.LastName &&
		dto.NationalCode == entity.NationalCode &&
		dto.Email == entity.Email &&
		dto.PhoneNumber == entity.PhoneNumber &&
		dto.BirthDate == entity.BirthDate &&
		(dto.Biography is null || dto.Biography == entity.Biography);
}