using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Translators;
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
			Books =
			[
				.. translator.BookTranslators.Select(ba => new BookSummaryDto
				{
					BookId = ba.BookId,
					BookName = ba.Book.BookName,
					ISBN = ba.Book.InternationalStandardBookNumber
				})
			],
			BookCount = translator.BookTranslators.Count,
			CreatedAt = translator.CreatedAt,
			UpdatedAt = translator.UpdatedAt
		};
	}
}