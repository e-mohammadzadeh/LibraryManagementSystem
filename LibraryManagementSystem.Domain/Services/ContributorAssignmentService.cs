using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Services;

public class ContributorAssignmentService
{
	public void AssignAuthorsToBook(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(authors);
		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0 && !book.BookAuthors.Any())
			throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);
		foreach (var author in authorList)
		{
			if (book.BookAuthors.Any(ba => ba.AuthorId == author.Id)) continue;
			var bookAuthor = new BookAuthor(book, author);
			book.AddBookAuthorInternal(bookAuthor);
		}

		book.UpdatedAt = DateTime.UtcNow;
	}


	public void AssignTranslatorsToBook(Book book, IEnumerable<Translator>? translators)
	{
		if (translators is null) return;
		var translatorList = translators.DistinctBy(t => t.Id).ToList();
		foreach (var translator in translatorList)
		{
			if (book.BookTranslators.Any(bt => bt.TranslatorId == translator.Id)) continue;

			var bookTranslator = new BookTranslator(book, translator);
			book.AddBookTranslatorInternal(bookTranslator);
		}

		book.UpdatedAt = DateTime.UtcNow;
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(authors);
		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException(Messages.BookRequiresAtLeastOneAuthor);

		var incomingAuthorIds = authorList.Select(a => a.Id).ToList();
		var authorsToRemove = book.BookAuthors.Where(ba => !incomingAuthorIds.Contains(ba.AuthorId)).ToList();

		foreach (var ba in authorsToRemove) RemoveAuthorFromBook(book, ba.AuthorId);
		AssignAuthorsToBook(book, authorList);
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(translators);
		var translatorList = translators.DistinctBy(t => t.Id).ToList();

		var incomingTranslatorIds = translatorList.Select(t => t.Id).ToList();
		var translatorsToRemove = book.BookTranslators.Where(bt => !incomingTranslatorIds.Contains(bt.TranslatorId)).ToList();

		foreach (var bt in translatorsToRemove) RemoveTranslatorFromBook(book, bt.TranslatorId);
		AssignTranslatorsToBook(book, translatorList);
	}


	public void DetachFromTranslators() 
	{
		foreach (var bookTranslator in _bookTranslators.ToList())
			RemoveTranslator(bookTranslator.TranslatorId);
		_bookTranslators.Clear();
		MarkAsUpdated();
	}




	private void RemoveAuthor(Book book, int authorId)
	{
		if (_bookAuthors.Count <= 1) throw new InvalidOperationException("A book must have at least one author.");
		RemoveAuthorInternal(authorId);
	}


	private void RemoveAuthorInternal(int authorId)
	{
		var bookAuthor = _bookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);
		if (bookAuthor is null) return;

		_bookAuthors.Remove(bookAuthor);
		bookAuthor.Author.RemoveBookAuthor(bookAuthor);
		MarkAsUpdated();
	}


	private void RemoveTranslator(int translatorId)
	{
		var bookTranslator = _bookTranslators.FirstOrDefault(ba => ba.TranslatorId == translatorId);

		if (bookTranslator is null) return;
		_bookTranslators.Remove(bookTranslator);
		bookTranslator.Translator.RemoveBookTranslator(bookTranslator);
		MarkAsUpdated();
	}
}