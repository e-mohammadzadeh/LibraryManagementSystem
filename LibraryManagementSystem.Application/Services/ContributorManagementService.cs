using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Application.Repositories.EFCore;
using LibraryManagementSystem.Application.Validators;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public sealed record ContributorMessages(
	string AddedSuccessfully,
	string UpdateFailed,
	string UpdatedSuccessfully,
	string RemoveFailed,
	string HasAssociatedBooks,
	string RemovedSuccessfully,
	string AlreadyActive,
	string RemovedNeedsRestore);


public sealed record ContributorAudit(
	string EntityName,
	AuditAction Created,
	AuditAction Updated,
	AuditAction Removed);


public abstract class ContributorManagementService<TEntity> where TEntity : Contributor, new()
{
	private readonly IContributorRepository<TEntity> _repository;
	private readonly PersonManagementService _personService;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;
	private readonly ContributorMessages _messages;
	private readonly ContributorAudit _audit;


	public ContributorManagementService(IContributorRepository<TEntity> repository,
		PersonManagementService personService, IAuthorizationService authorization, IAuditLogManagementService auditLog,
		ContributorMessages messages, ContributorAudit audit)
	{
		_repository = repository;
		_personService = personService;
		_authorization = authorization;
		_auditLog = auditLog;
		_messages = messages;
		_audit = audit;
	}


	protected abstract IEnumerable<BookDto> GetBooks(Guid contributorId);


	protected ServiceResult<ContributorDto> AddContributor(ContributorDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var dateValidation = Validator.ContributorDateValidator(dto.BirthDate, dto.DeathDate);
		if (!dateValidation.IsValid) return ServiceResult<ContributorDto>.Fail(dateValidation.ErrorMessage!);

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
		return ServiceResult<ContributorDto>.Ok(entity.ToDto(), _messages.AddedSuccessfully);
	}


	protected ServiceResult<ContributorDto> UpdatedContributor(ContributorDto dto, Guid? updatedBy)
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

		_auditLog.Record(_audit.Updated, _audit.EntityName, entity.Id, $"{_audit.EntityName} and/or Person information updated.");
		var refreshed = _repository.GetById(dto.Id, EntityFilter.Active) ?? entity;
		return ServiceResult<ContributorDto>.Ok(refreshed.ToDto(), _messages.UpdatedSuccessfully);
	}
}