using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
	public Book(string internationalStandardBookNumber, string title, DateOnly publishDate, int totalCopies,
		Genre genre, string publisher, string? description)
	{
		Id = Guid.CreateVersion7();
		InternationalStandardBookNumber = internationalStandardBookNumber;
		Title = title;


		PublishDate = publishDate;
		var copies = ValidateTotalCopies(totalCopies);
		AvailableCopies = copies;
		TotalCopies = copies;
		Genre = genre;
		Publisher = publisher;
		Description = description;
		CreatedAt = DateTime.UtcNow;
		IsRemoved = false;
	}


	//TODO	(SQL Server)	When switch into SQL Server, IDs will generate by SQL Server itself and should remove static ones
	public Guid Id { get; private set; }
	public string Title { get; private set; }
	public string InternationalStandardBookNumber { get; private set; }
	private readonly List<BookAuthor> _bookAuthors = [];
	private readonly List<BookTranslator> _bookTranslators = [];
	public DateOnly PublishDate { get; private set; }
	public Genre Genre { get; private set; }
	public string Publisher { get; private set; }
	public int TotalCopies { get;  set; }
	public int AvailableCopies { get;  set; }
	public string? Description { get; private set; }
	public DateTime CreatedAt { get; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }


	public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

	public IReadOnlyList<BookTranslator> BookTranslators => _bookTranslators.AsReadOnly();


	private static int ValidateTotalCopies(int totalCopies)
	{
		return totalCopies > 0 ? totalCopies : throw new ArgumentException("Invalid total copy value.Please try again");
	}


	public void AddAuthor(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		if (_bookAuthors.Any(ba => ba.AuthorId == author.Id)) return;

		_bookAuthors.Add(new BookAuthor(this, author));
		UpdatedAt = DateTime.UtcNow;
	}


	public void AddTranslator(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

		if (_bookTranslators.Any(bt => bt.TranslatorId == translator.Id)) return;

		_bookTranslators.Add(new BookTranslator(this, translator));
		UpdatedAt = DateTime.UtcNow;
	}


	public void RemoveAuthor(Guid authorId)
	{
		if (_bookAuthors.Count <= 1)
		{
			throw new InvalidOperationException(
				Messages.BookRequiresAtLeastOneAuthor);
		}

		var bookAuthor = _bookAuthors
			.FirstOrDefault(ba => ba.AuthorId == authorId);

		if (bookAuthor is null) return;

		_bookAuthors.Remove(bookAuthor);
		UpdatedAt = DateTime.UtcNow;
	}


	public void RemoveTranslator(Guid translatorId)
	{
		var bookTranslator = _bookTranslators
			.FirstOrDefault(bt => bt.TranslatorId == translatorId);

		if (bookTranslator is null) return;

		_bookTranslators.Remove(bookTranslator);
		UpdatedAt = DateTime.UtcNow;
	}
	

	//public void BorrowCopy()
	//{
	//	if (AvailableCopies <= 0) throw new InvalidOperationException("No copies are available.");
	//	AvailableCopies--;
	//	//TODO	(Web API)	Raise an event: a signal to the rest of the system that says "this book is now out of stock"
	//}


	//public void ReturnCopy()
	//{
	//	if (AvailableCopies >= TotalCopies)
	//		throw new InvalidOperationException("Cannot return a copy because all copies are already in the library.");

	//	AvailableCopies++;
	//}
}