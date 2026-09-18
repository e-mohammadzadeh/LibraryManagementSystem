using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Services;

public class ContributorAssignmentService
{
	public void AssignAuthorsToBook(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(authors);
		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException("A book must have at least one author.");
		foreach (var author in authorList)
		{
			if (_bookAuthors.Any(ba => ba.AuthorId == author.Id)) return;

			var bookAuthor = new BookAuthor(book, author);
			_bookAuthors.Add(bookAuthor);
		}
	}


	public void AssignTranslatorToBook(Book book, IEnumerable<Translator>? translators)
	{
		if (translators is not null)
		{
			var translatorList = translators.DistinctBy(t => t.Id).ToList();
			foreach (var translator in translatorList)
			{
				ArgumentNullException.ThrowIfNull(translator);
				if (_bookTranslators.Any(ba => ba.TranslatorId == translator.Id)) return;

				var bookTranslator = new BookTranslator(book, translator);
				_bookTranslators.Add(bookTranslator);
			}
		}
	}


	public void ReplaceAuthors(Book book, IEnumerable<Author> authors)
	{
		ArgumentNullException.ThrowIfNull(authors);
		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count == 0) throw new ArgumentException("A book must have at least one author.");

		foreach (var bookAuthor in _bookAuthors.ToList()) RemoveAuthorInternal(bookAuthor.AuthorId);
		foreach (var author in authorList) AssignAuthorsToBook(author);
		book.UpdatedAt = DateTime.UtcNow;

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



	public void DetachFromTranslators()
	{
		foreach (var bookTranslator in _bookTranslators.ToList()) RemoveTranslator(bookTranslator.TranslatorId);
		_bookTranslators.Clear();
		MarkAsUpdated();
	}


	public void ReplaceTranslators(Book book, IEnumerable<Translator> translators)
	{
		ArgumentNullException.ThrowIfNull(translators);
		foreach (var bookTranslator in _bookTranslators.ToList()) RemoveTranslator(bookTranslator.TranslatorId);
		foreach (var translator in translators.DistinctBy(t => t.Id)) AddTranslator(translator);
		book.UpdatedAt = DateTime.UtcNow;
	}
}