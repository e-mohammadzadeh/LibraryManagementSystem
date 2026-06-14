namespace LibraryManagementSystem;

public class Book
{
	public Book()
	{
	}

	public int BookId { get; set; }
	public string InternationalStandardBookNumber { get; set; }
	public required string BookName { get; set; }
	public required Author Author { get; set; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }

	public void BorrowCopy()
	{
		if (AvailableCopies <= 0)
		{
			throw new InvalidOperationException("No copies are available.");
		}
		AvailableCopies--;
	}


	public void ReturnCopy()
	{
		if (AvailableCopies == TotalCopies)
		{
			throw new InvalidOperationException("Cannot return a copy because all copies are already in the library.");
		}
		AvailableCopies++;
	}
}