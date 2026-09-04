using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Common;

public static class BookUpdateAuditDetailsBuilder
{
	public static string? BuildBookUpdateAuditDetails(Book book, UpdateBookDto dto,
		IReadOnlyList<Author>? resolvedAuthors = null, IReadOnlyList<Translator>? resolvedTranslators = null)
	{
		var changes = new List<string>();

		if (dto.BookName is not null && dto.BookName != book.BookName)
			changes.Add($"Changed book name from '{book.BookName}' to '{dto.BookName}'.");

		if (dto.ISBN is not null && dto.ISBN != book.InternationalStandardBookNumber)
			changes.Add($"Changed ISBN from '{book.InternationalStandardBookNumber}' to '{dto.ISBN}'.");

		if (dto.PublishDate is not null && dto.PublishDate != book.PublishDate)
			changes.Add(
				$"Changed publish date from '{book.PublishDate:yyyy-MM-dd}' to '{dto.PublishDate.Value:yyyy-MM-dd}'.");

		if (dto.TotalCopies is not null && dto.TotalCopies != book.TotalCopies)
			changes.Add($"Changed total copies from '{book.TotalCopies}' to '{dto.TotalCopies}'.");

		if (dto.GenreId is not null && dto.GenreId != (int)book.Genre)
			changes.Add($"Changed genre from '{book.Genre}' to '{(Genre)dto.GenreId.Value}'.");

		// If Genre is stored as enum on DTO instead of GenreId:
		// if (dto.Genre is not null && dto.Genre != book.Genre)
		//     changes.Add($"Changed genre from '{book.Genre}' to '{dto.Genre}'.");

		if (dto.Publisher is not null && dto.Publisher != book.Publisher)
			changes.Add($"Changed publisher from '{book.Publisher}' to '{dto.Publisher}'.");

		if (dto.Description is not null && dto.Description != book.Description)
		{
			var oldDesc = string.IsNullOrWhiteSpace(book.Description) ? "None" : book.Description;
			var newDesc = string.IsNullOrWhiteSpace(dto.Description) ? "None" : dto.Description;
			changes.Add($"Changed description from '{oldDesc}' to '{newDesc}'.");
		}

		// Authors
		if (resolvedAuthors is not null)
		{
			var oldAuthors = book.BookAuthors
				.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}")
				.OrderBy(name => name)
				.ToList();

			var newAuthors = resolvedAuthors
				.Select(a => $"{a.FirstName} {a.LastName}")
				.OrderBy(name => name)
				.ToList();

			if (!oldAuthors.SequenceEqual(newAuthors))
			{
				var oldText = oldAuthors.Count > 0 ? string.Join(", ", oldAuthors) : "None";
				var newText = newAuthors.Count > 0 ? string.Join(", ", newAuthors) : "None";
				changes.Add($"Changed authors from '{oldText}' to '{newText}'.");
			}
		}

		// Translators
		if (resolvedTranslators is not null)
		{
			var oldTranslators = book.BookTranslators
				.Select(bt => $"{bt.Translator.FirstName} {bt.Translator.LastName}")
				.OrderBy(name => name)
				.ToList();

			var newTranslators = resolvedTranslators
				.Select(t => $"{t.FirstName} {t.LastName}")
				.OrderBy(name => name)
				.ToList();

			if (!oldTranslators.SequenceEqual(newTranslators))
			{
				var oldText = oldTranslators.Count > 0 ? string.Join(", ", oldTranslators) : "None";
				var newText = newTranslators.Count > 0 ? string.Join(", ", newTranslators) : "None";
				changes.Add($"Changed translators from '{oldText}' to '{newText}'.");
			}
		}

		return changes.Count > 0 ? string.Join(" ", changes) : null;
	}
}