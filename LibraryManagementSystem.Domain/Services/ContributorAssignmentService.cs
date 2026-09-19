using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Services;

public class ContributorAssignmentService
{
	public void AssignAuthorsToBook(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		foreach (var author in authorList) book.AddAuthor(author);
	}


	public void AssignTranslatorsToBook(Book book, IEnumerable<Translator>? translators)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (translators is null) return;
		foreach (var translator in translators.DistinctBy(t => t.Id)) book.AddTranslator(translator);
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(authors);

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingIds = authorList.Select(a => a.Id).ToHashSet();
		var existingIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();

		foreach (var authorId in existingIds.Except(incomingIds)) book.RemoveAuthor(authorId);
		foreach (var author in authorList) book.AddAuthor(author);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(book);
		ArgumentNullException.ThrowIfNull(translators);

		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		var incomingIds = translatorList.Select(t => t.Id).ToHashSet();
		var existingIds = book.BookTranslators.Select(bt => bt.TranslatorId).ToHashSet();

		foreach (var translatorId in existingIds.Except(incomingIds)) book.RemoveTranslator(translatorId);
		foreach (var translator in translatorList) book.AddTranslator(translator);
	}


	public void DetachFromTranslators(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		foreach (var translatorId in book.BookTranslators.Select(bt => bt.TranslatorId).ToList())
			book.RemoveTranslator(translatorId);
	}
}