using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Mapping;

public static class TranslatorMapper
{
	public static ContributorDto ToDto(this Translator translator)
	{
		return new ContributorDto
		{
			Id = translator.Id,
			FirstName = translator.FirstName,
			LastName = translator.LastName,
			NationalCode = translator.NationalCode,
			Email = translator.Email,
			PhoneNumber = translator.PhoneNumber,
			BirthDate = translator.BirthDate,
			Biography = translator.Biography,
			CreatedAt = translator.CreatedAt,
			UpdatedAt = translator.UpdatedAt
		};
	}
}