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

public sealed record ContributorMessages(
	string AddedSuccessfully,
	string UpdateFailed,
	string UpdatedSuccessfully,
	string RemoveFailed,
	string RestoredSuccessfully,
	string RestoreFailed,
	string HasAssociatedBooks,
	string RemovedSuccessfully,
	string AlreadyActive,
	string RemovedNeedsRestore);


public sealed record ContributorAudit(
	string EntityName,
	AuditAction Created,
	AuditAction Updated,
	AuditAction Removed,
	AuditAction Restored);


public abstract class ContributorManagementService<TEntity> where TEntity : Contributor, new()
{
	private readonly IContributorRepository<TEntity> _repository;
	private readonly PersonManagementService _personService;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;
	private readonly ContributorMessages _messages;
	private readonly ContributorAudit _audit;
	private readonly IUnitOfWork _unitOfWork;


	public ContributorManagementService(IContributorRepository<TEntity> repository,
		PersonManagementService personService, IAuthorizationService authorization, IAuditLogManagementService auditLog,
		ContributorMessages messages, ContributorAudit audit, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_personService = personService;
		_authorization = authorization;
		_auditLog = auditLog;
		_messages = messages;
		_audit = audit;
		_unitOfWork = unitOfWork;
	}


	protected abstract IEnumerable<BookDto> GetBooks(Guid contributorId);


	protected ServiceResult<ContributorDto> AddContributor(ContributorDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var dateValidation = Validator.ContributorDateValidator(dto.BirthDate, dto.DeathDate);
		if (!dateValidation.IsValid) return ServiceResult<ContributorDto>.Fail(dateValidation.ErrorMessage!);

		using var tx = _unitOfWork.Begin();

		var personResult = _personService.ResolveOrCreatePerson(dto);
		if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message!);
		var person = personResult.Data!;

		var existing = _repository.GetByPersonId(person.Id, EntityFilter.All);
		if (existing is not null)
			return ServiceResult<ContributorDto>.Fail(existing.IsRemoved
				? _messages.RemovedNeedsRestore
				: _messages.AlreadyActive);

		var entity = new TEntity
		{
			PersonId = person.Id,
			Person = person,
			DeathDate = dto.DeathDate,
			Biography = dto.Biography
		};

		_repository.Add(entity);
		_auditLog.Record(_audit.Created, _audit.EntityName, entity.Id, $"{_audit.EntityName} role created.");

		tx.Commit();
		return ServiceResult<ContributorDto>.Ok(entity.ToDto(), _messages.AddedSuccessfully);
	}


	protected ServiceResult<ContributorDto> UpdateContributor(ContributorDto dto, Guid? updatedBy)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var entity = _repository.GetByPersonId(dto.Id, EntityFilter.Active);
		if (entity is null) return ServiceResult<ContributorDto>.Fail(_messages.UpdateFailed);

		var person = entity.Person;
		var personChanged = person.FirstName != dto.FirstName ||
		                    person.LastName != dto.LastName ||
		                    person.NationalCode != dto.NationalCode ||
		                    !Equals(person.Email, dto.Email) ||
		                    !Equals(person.PhoneNumber, dto.PhoneNumber) ||
		                    person.BirthDate != dto.BirthDate;

		var roleChanged = entity.DeathDate != dto.DeathDate ||
		                  entity.Biography != dto.Biography;

		if (!personChanged && !roleChanged)
			return ServiceResult<ContributorDto>.Warning(entity.ToDto(), Messages.NoChangesDetected);

		using var tx = _unitOfWork.Begin();

		if (personChanged)
		{
			var personResult = _personService.UpdatePerson(person.Id, dto, updatedBy);
			if (!personResult.IsSuccess) return ServiceResult<ContributorDto>.Fail(personResult.Message!);
		}

		if (roleChanged)
		{
			entity.DeathDate = dto.DeathDate;
			entity.Biography = dto.Biography;
			entity.UpdatedByUserId = updatedBy;
			_repository.Update(entity);
		}

		_auditLog.Record(_audit.Updated, _audit.EntityName, entity.Id,
			$"{_audit.EntityName} and/or Person information updated.");
		var refreshed = _repository.GetById(dto.Id, EntityFilter.Active) ?? entity;

		tx.Commit();
		return ServiceResult<ContributorDto>.Ok(refreshed.ToDto(), _messages.UpdatedSuccessfully);
	}


	protected ServiceResult<ContributorDto> RemoveContributor(Guid id, Guid? removedBy)
	{
		var entity = _repository.GetById(id, EntityFilter.Active);
		if (entity is null) return ServiceResult<ContributorDto>.Fail(_messages.RemoveFailed);

		if (GetBooks(id).Any()) return ServiceResult<ContributorDto>.Fail(_messages.HasAssociatedBooks);

		using var tx = _unitOfWork.Begin();

		entity.UpdatedByUserId = removedBy;
		_repository.Remove(entity);

		var removed = _repository.GetById(id, EntityFilter.Removed);
		if (removed is null) return ServiceResult<ContributorDto>.Fail(_messages.RemoveFailed);

		_auditLog.Record(_audit.Removed, _audit.EntityName, id, $"{_audit.EntityName} role removed.");

		tx.Commit();
		return ServiceResult<ContributorDto>.Ok(removed.ToDto(), _messages.RemovedSuccessfully);
	}


	protected ServiceResult<ContributorDto> RestoreContributor(Guid id, Guid? restoredBy)
	{
		var entity = _repository.GetById(id, EntityFilter.Removed);
		if (entity is null) return ServiceResult<ContributorDto>.Fail(_messages.RestoreFailed);

		using var tx = _unitOfWork.Begin();

		_repository.Restore(entity, restoredBy);

		var restored = _repository.GetById(id, EntityFilter.Active);
		if (restored is null) return ServiceResult<ContributorDto>.Fail(_messages.RestoreFailed);

		_auditLog.Record(_audit.Restored, _audit.EntityName, id, $"{_audit.EntityName} role restored.");

		tx.Commit();
		return ServiceResult<ContributorDto>.Ok(restored.ToDto(), _messages.RestoredSuccessfully);
	}



	protected IReadOnlyList<ContributorDto> SearchContributors(string searchItem, ContributorSearchField field)
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

		return
		[
			.. _repository.GetAll(EntityFilter.Active)
				.Where(c =>
				{
					var value = field switch
					{
						ContributorSearchField.Name => $"{c.Person.FirstName} {c.Person.LastName}",
						ContributorSearchField.NationalCode => c.Person.NationalCode,
						ContributorSearchField.Email => c.Person.Email,
						ContributorSearchField.PhoneNumber => c.Person.PhoneNumber,
						_ => throw new ArgumentOutOfRangeException(nameof(field))
					};

					return value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(c => c.ToDto())
		];
	}


	protected IReadOnlyList<ContributorDto> GetAllContributors(
		ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var items = _repository.GetAll(EntityFilter.Active).Select(c => c.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => dto => dto.FirstName,
			ContributorSortField.LastName => dto => dto.LastName,
			ContributorSortField.FullName => dto => $"{dto.FirstName} {dto.LastName}",
			ContributorSortField.NationalCode => dto => dto.NationalCode,
			ContributorSortField.Email => dto => dto.Email,
			ContributorSortField.BirthDate => dto => dto.BirthDate,
			ContributorSortField.BookCount => dto => GetBooks(dto.Id).Count(),
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		return
		[
			.. sortDirection == SortDirection.Ascending
				? items.OrderBy(keySelector)
				: items.OrderByDescending(keySelector)
		];
	}


	protected IReadOnlyList<ContributorDto> GetRemovedContributors()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];
		return [.._repository.GetAll(EntityFilter.Removed).Select(c => c.ToDto())];
	}


	protected IReadOnlyList<BookDto> GetBooksOf(Guid contributorId) => [.. GetBooks(contributorId)];
}