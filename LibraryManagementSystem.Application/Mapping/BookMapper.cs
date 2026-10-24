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
			ISBN = book.ISBN,
			Authors = [.. book.BookAuthors.Select(ba => ba.Author.ToDto())],
			Translators = [.. book.BookTranslators.Select(bt => bt.Translator.ToDto())],
			PublishDate = book.PublishDate,
			Genre = book.Genre,
			Publisher = book.Publisher,
			OriginalLanguage = book.OriginalLanguage,
			TotalCopies = book.TotalCopies,
			AvailableCopies = book.AvailableCopies,
			Description = book.Description!,
			CreatedAt = book.CreatedAt,
			UpdatedAt = book.UpdatedAt ?? default,
			UpdatedByUserId = book.UpdatedByUserId ?? Guid.Empty
		};
	}
}


