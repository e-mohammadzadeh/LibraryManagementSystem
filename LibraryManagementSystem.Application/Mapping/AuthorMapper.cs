using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Mapping;

public static class AuthorMapper
{
	public static ContributorDto ToDto(this Author author)
	{
		return new ContributorDto
		{
			Id = author.Id,
			FirstName = author.FirstName,
			LastName = author.LastName,
			NationalCode = author.NationalCode,
			Email = author.Email,
			PhoneNumber = author.PhoneNumber,
			BirthDate = author.BirthDate,
			Biography = author.Biography,
			CreatedAt = author.CreatedAt,
			UpdatedAt = author.UpdatedAt
		};
	}
}