using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.Domain;

public class Book
{
	public Book(string internationalStandardBookNumber, string bookName, Author author, DateOnly publishDate,
		int totalCopies, Genre genre, string? description)
	{
		BookId = _nextBookId++;
		InternationalStandardBookNumber = internationalStandardBookNumber;
		BookName = bookName;
		Author = author;
		PublishDate = publishDate;
		AvailableCopies = ValidateTotalCopies(totalCopies);
		TotalCopies = ValidateTotalCopies(totalCopies);
		Genre = genre;
		Description = description;
	}


	//TODO	When switch into SQL Server, IDs will generate by SQL Server itself and should remove static ones
	private static int _nextBookId = 1;
	public int BookId { get; private set; }
	public string BookName { get; set; }
	public string InternationalStandardBookNumber { get; set; }
	public Author Author { get; private set; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	public int TotalCopies { get; private set; }
	private int AvailableCopies { get; set; }
	public string? Description { get; set; }


	public void Update(string? bookName, string? isbn, Author? author, DateOnly? publishDate, Genre? genreId,
		int? totalCopies, int dif, string? description)
	{
		BookName = bookName ?? BookName;
		InternationalStandardBookNumber = isbn ?? InternationalStandardBookNumber;
		Author = author ?? Author;
		PublishDate = publishDate ?? PublishDate;
		Genre = genreId ?? Genre;
		TotalCopies = totalCopies ?? TotalCopies;
		AvailableCopies += dif;
		Description = description ?? Description;
	}


	private static int ValidateTotalCopies(int totalCopies)
	{
		return totalCopies > 0 ? totalCopies : throw new ArgumentException("Invalid total copy value.Please try again");
	}


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