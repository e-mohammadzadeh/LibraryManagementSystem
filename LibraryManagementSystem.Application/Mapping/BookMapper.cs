using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class BookMapper
{
	public static BookDto ToDto(this Book book)
	{
		return new BookDto
		{
			BookId = book.Id,
			BookName = book.BookName,
			ISBN = book.InternationalStandardBookNumber,
			Authors = [.. book.BookAuthors.Select(ba => ba.Author.ToDto())],
			Translators = [.. book.BookTranslators.Select(bt => bt.Translator.ToDto())],
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
			BookName = book.BookName,
			ISBN = book.InternationalStandardBookNumber,
			AvailableCopies = book.AvailableCopies
		};
	}
}


