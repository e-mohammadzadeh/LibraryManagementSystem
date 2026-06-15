namespace LibraryManagementSystem;

public class Book
{
	public Book(string bookName, Author author, string internationalStandardBookNumber, int totalCopies)
	{
		BookId = _nextBookId++;
		BookName = bookName;
		Author = author;
		InternationalStandardBookNumber = internationalStandardBookNumber;
		TotalCopies = totalCopies;
		AvailableCopies = totalCopies;
	}

	private static int _nextBookId;
	public int BookId { get; private set; }
	public string InternationalStandardBookNumber { get; set; }
	public required string BookName { get; set; }
	public required Author Author { get; set; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	private int TotalCopies { get; init; }  //TODO	should check init value to be positive
	private int AvailableCopies { get; set; }
	public string? Description { get; set; }

	public void BorrowCopy()
	{
		if (AvailableCopies <= 0)
		{
			throw new InvalidOperationException("No copies are available.");
		}
		AvailableCopies--;
		//TODO	Raise an event: a signal to the rest of the system that says "this book is now out of stock"
	}


	public void ReturnCopy()
	{
		if (AvailableCopies >= TotalCopies)
		{
			throw new InvalidOperationException("Cannot return a copy because all copies are already in the library.");
		}
		AvailableCopies++;
	}
}