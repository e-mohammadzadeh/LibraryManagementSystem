using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Services;

public abstract class ContributorManagementService<TEntity> where TEntity : Contributor, new()
{
	protected readonly IContributorRepository<TEntity> Repository;


	protected ContributorManagementService(IContributorRepository<TEntity> repository) { Repository = repository; }


	protected ServiceResult<ContributorDto> AddContributor(ContributorDto dto, string duplicateNationalCodeMessage,
		string duplicateEmailMessage, string duplicatePhoneNumberMessage, string successMessage)
	{
		if (Repository.Exists(dto.NationalCode, c => c.NationalCode))
			return ServiceResult<ContributorDto>.Fail(duplicateNationalCodeMessage);

		if (Repository.Exists(dto.Email, c => c.Email))
			return ServiceResult<ContributorDto>.Fail(duplicateEmailMessage);

		if (Repository.Exists(dto.PhoneNumber, c => c.PhoneNumber))
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
		string updateFailedMessage, string duplicateNationalCodeMessage, string duplicateEmailMessage,
		string duplicatePhoneNumberMessage, string successMessage)
	{
		var entity = Repository.GetById(dto.Id, EntityFilter.Active);
		if (entity is null || entity.IsRemoved) return ServiceResult<ContributorDto>.Fail(updateFailedMessage);

		if (IsNoOp(entity, dto))
			return ServiceResult<ContributorDto>.Warning(entity.ToDto(), Messages.NoChangesDetected);

		if (Repository.Exists(dto.NationalCode, c => c.NationalCode, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicateNationalCodeMessage);

		if (Repository.Exists(dto.Email, c => c.Email, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicateEmailMessage);

		if (Repository.Exists(dto.PhoneNumber, c => c.PhoneNumber, dto.Id))
			return ServiceResult<ContributorDto>.Fail(duplicatePhoneNumberMessage);

		entity.FirstName = dto.FirstName;
		entity.LastName = dto.LastName;
		entity.NationalCode = dto.NationalCode;
		entity.Email = dto.Email;
		entity.PhoneNumber = dto.PhoneNumber;
		entity.BirthDate = dto.BirthDate;
		entity.Biography = dto.Biography;
		entity.UpdatedByUserId = updatedBy;

		Repository.Update(entity);
		return ServiceResult<ContributorDto>.Ok(entity.ToDto(), successMessage);
	}






	private static bool IsNoOp(TEntity entity, ContributorDto dto) =>
		dto.FirstName == entity.FirstName &&
		dto.LastName == entity.LastName &&
		dto.NationalCode == entity.NationalCode &&
		dto.Email == entity.Email &&
		dto.PhoneNumber == entity.PhoneNumber &&
		dto.BirthDate == entity.BirthDate &&
		(dto.Biography is null || dto.Biography == entity.Biography);
}