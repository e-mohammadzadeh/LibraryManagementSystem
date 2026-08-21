using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
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
			Books =
			[
				.. author.BookAuthors.Select(ba => new BookSummaryDto
				{
					BookId = ba.BookId,
					BookName = ba.Book.BookName,
					ISBN = ba.Book.InternationalStandardBookNumber
				})
			],
			CreatedAt = author.CreatedAt,
			UpdatedAt = author.UpdatedAt
		};
	}
}