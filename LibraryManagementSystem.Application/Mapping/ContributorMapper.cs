using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Mapping;

public static class ContributorMapper
{
	public static ContributorDto ToDto<TEntity>(this TEntity entity) where TEntity : Contributor
	{
		ArgumentNullException.ThrowIfNull(entity);
		var person = entity.Person ??
		             throw new InvalidOperationException(
			             "Contributor.Person was ot loaded. Include(Person> is required.");

		return new ContributorDto
		{
			Id = entity.Id,
			PersonId = entity.PersonId,
			FirstName = person.FirstName,
			LastName = person.LastName,
			NationalCode = person.NationalCode,
			Email = person.Email,
			PhoneNumber = person.PhoneNumber,
			BirthDate = person.BirthDate,
			DeathDate = entity.DeathDate,
			Biography = entity.Biography,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}