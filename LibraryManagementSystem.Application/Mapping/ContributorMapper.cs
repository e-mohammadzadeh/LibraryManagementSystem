using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Mapping;

public static class ContributorMapper
{
	public static ContributorDto ToDto<TEntity>(this TEntity entity) where TEntity : Contributor 
	{
		if (entity == null) return null;

		return new ContributorDto
		{
			Id = entity.Id,
			FirstName = entity.FirstName,
			LastName = entity.LastName,
			NationalCode = entity.NationalCode,
			Email = entity.Email,
			PhoneNumber = entity.PhoneNumber,
			BirthDate = entity.BirthDate,
			DeathDate = entity.DeathDate,
			Biography = entity.Biography,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}