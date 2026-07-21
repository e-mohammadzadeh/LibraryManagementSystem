using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
	public Book(string internationalStandardBookNumber, string bookName, IEnumerable<Author> authors,
		Translator? translator, DateOnly publishDate, int totalCopies, Genre genre, string publisher,
		string? description)
	{
		BookId = ++_nextBookId;
		InternationalStandardBookNumber = internationalStandardBookNumber;
		BookName = bookName;

		var authorList = authors.DistinctBy(a => a.Id).ToList();
		if (authorList.Count is 0 || authorList is null)
			throw new ArgumentException(ValidationMessages.BookRequiresAtLeastOneAuthor);
		foreach (var author in authorList) AddAuthor(author);

		Translator = translator;
		PublishDate = publishDate;
		var copies = ValidateTotalCopies(totalCopies);
		AvailableCopies = copies;
		TotalCopies = copies;
		Genre = genre;
		Publisher = publisher;
		Description = description;
	}


	//TODO	When switch into SQL Server, IDs will generate by SQL Server itself and should remove static ones
	private static int _nextBookId;
	public int BookId { get; private set; }
	public string BookName { get; set; }
	public string InternationalStandardBookNumber { get; set; }
	private readonly List<BookAuthor> _bookAuthors = new();
	public Translator? Translator { get; set; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	public string Publisher { get; set; }
	public int TotalCopies { get; private set; }
	public int AvailableCopies { get; private set; }
	public string? Description { get; set; }


	public void AddAuthor(Author author)
	{
		if (author is null) throw new ArgumentNullException(nameof(author));

		if (_bookAuthors.Any(ba => ba.AuthorId == author.Id)) return;

		var bookAuthor = new BookAuthor(this, author);
		_bookAuthors.Add(bookAuthor);
		author.AddBookAuthor(bookAuthor);
	}


	public void RemoveAuthor(int authorId)
	{
		var bookAuthor = _bookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);

		if (bookAuthor is null) return;
		_bookAuthors.Remove(bookAuthor);
		bookAuthor.Author.RemoveBookAuthor(bookAuthor);
	}


	public bool Update(string? bookName, string? isbn, DateOnly? publishDate, Genre? genreId, string? publisher,
		int? totalCopies, string? description)
	{
		if (totalCopies != null)
		{
			var difference = totalCopies.Value - TotalCopies;
			if (AvailableCopies + difference < 0) return false;
			TotalCopies = totalCopies.Value;
			AvailableCopies += difference;
		}

		BookName = bookName ?? BookName;
		InternationalStandardBookNumber = isbn ?? InternationalStandardBookNumber;
		PublishDate = publishDate ?? PublishDate;
		Genre = genreId ?? Genre;
		Publisher = publisher ?? Publisher;
		Description = description ?? Description;
		return true;
	}


	private static int ValidateTotalCopies(int totalCopies)
	{
		return totalCopies > 0 ? totalCopies : throw new ArgumentException("Invalid total copy value.Please try again");
	}


	//public void ChangeAuthor(Author? newAuthor)
	//{
	//	if (ReferenceEquals(Author, newAuthor)) return;

	//	Author?.Books.Remove(this);

	//	Author = newAuthor;
	//	newAuthor?.Books.Add(this);
	//}


	public void ChangeTranslator(Translator? newTranslator)
	{
		if (ReferenceEquals(Translator, newTranslator)) return;

		Translator?.Books.Remove(this);

		Translator = newTranslator;
		newTranslator?.Books.Add(this);
	}


	public void BorrowCopy()
	{
		AvailableCopies--;
		//TODO	Raise an event: a signal to the rest of the system that says "this book is now out of stock"
		//LoanService.BorrowBook(memberId, bookId)
		//	→ find Member → fail if not found
		//	→ find Book → fail if not found
		//	→ call book.BorrowCopy() → throws if no copies available
		//	→ create new Loan(book, member, dueDate)
		//	→ add Loan to _loans list
		//	→ return ServiceResult<Loan>.Ok(loan)
	}


	public void ReturnCopy()
	{
		if (AvailableCopies >= TotalCopies)
			throw new InvalidOperationException("Cannot return a copy because all copies are already in the library.");

		AvailableCopies++;
		//LoanService.ReturnBook(loanId)
		//	→ find Loan → fail if not found
		//	→ check loan not already returned → fail if already closed
		//	→ call loan.Book.ReturnCopy()
		//	→ set loan.ReturnDate = today
		//	→ calculate if overdue
		//	→ return ServiceResult<Loan>.Ok(loan)
	}


	public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

	public bool CanBeRemoved() { return TotalCopies == AvailableCopies; }
}