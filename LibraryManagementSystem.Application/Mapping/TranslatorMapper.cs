using LibraryManagementSystem.Application.DTOs.Translator;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class TranslatorMapper
{
	public static TranslatorDto ToDto(this Translator translator)
	{
		return new TranslatorDto
		{
			Id = translator.Id,
			FirstName = translator.FirstName,
			LastName = translator.LastName,
			NationalCode = translator.NationalCode,
			Email = translator.Email,
			PhoneNumber = translator.PhoneNumber,
			BirthDate = translator.BirthDate,
			BookCount = translator.BookTranslators.Count,
			CreatedAt = translator.CreatedAt,
			UpdatedAt = translator.UpdatedAt
		};
	}
}