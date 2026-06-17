using LibraryManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Domain;

public class Book
{
	public Book(string internationalStandardBookNumber, string bookName, Author author, DateOnly publishDate,
		int totalCopies)
	{
		BookId = _nextBookId++;
		InternationalStandardBookNumber = internationalStandardBookNumber;
		BookName = bookName;
		Author = author;
		PublishDate = publishDate;
		AvailableCopies = ValidateTotalCopies(totalCopies);
		TotalCopies = ValidateTotalCopies(totalCopies);
	}

	//TODO	When switch into SQL Server, IDs will generate by SQL Server itself and should remove static ones
	private static int _nextBookId;
	public int BookId { get; private set; }
	public string InternationalStandardBookNumber { get; set; }
	public required string BookName { get; set; }
	public required Author Author { get; init; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	private int TotalCopies { get; init; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }


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
		Loan loan = new Loan(); // Create Loan
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