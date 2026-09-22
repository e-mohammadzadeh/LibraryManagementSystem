using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Books;

namespace LibraryManagementSystem.Application.Mapping;

public static class BookMapper
{
	public static BookDto ToDto(this Book book)
	{
		return new BookDto
		{
			Id = book.Id,
			Title = book.Title,
			ISBN = book.InternationalStandardBookNumber,
			PublishDate = book.PublishDate,
			Genre = book.Genre.ToString(),
			Publisher = book.Publisher,
			TotalCopies = book.TotalCopies,
			AvailableCopies = book.AvailableCopies,
			Description = book.Description,
			CreatedAt = book.CreatedAt,
			UpdatedAt = book.UpdatedAt
		};
	}

	public static BookSummaryDto ToSummaryDto(this Book book)
	{
		return new BookSummaryDto
		{
			BookId = book.Id,
			BookName = book.Title,
			ISBN = book.InternationalStandardBookNumber,
			AvailableCopies = book.AvailableCopies
		};
	}
}


