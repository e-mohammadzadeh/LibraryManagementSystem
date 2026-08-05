using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class AuthorMapper
{
	public static AuthorDto ToDto(this Author author)
	{
		return new AuthorDto
		{
			Id = author.Id,
			FirstName = author.FirstName,
			LastName = author.LastName,
			NationalCode = author.NationalCode,
			Email = author.Email,
			PhoneNumber = author.PhoneNumber,
			BirthDate = author.BirthDate,
			Biography = author.Biography,
			BookCount = author.BookAuthors.Count,
			CreatedAt = author.CreatedAt,
			UpdatedAt = author.UpdatedAt
		};
	}
}