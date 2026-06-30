using System.Diagnostics.CodeAnalysis;
using LibraryManagementSystem.Enums;
using LibraryManagementSystem.Helpers;

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


	public bool Update(string? bookName, string? isbn, DateOnly? publishDate, Genre? genreId,
		int? totalCopies, string? description)
	{
		if (totalCopies != null)
		{
			var difference = totalCopies.Value - TotalCopies;
			if (AvailableCopies + difference < 0)
			{
				ConsoleHelper.ShowError(
					"Cannot update total copies because it would result in negative available copies.");

				return false;
			}

			TotalCopies = totalCopies.Value;
			AvailableCopies += difference;
		}

		BookName = bookName ?? BookName;
		InternationalStandardBookNumber = isbn ?? InternationalStandardBookNumber;
		PublishDate = publishDate ?? PublishDate;
		Genre = genreId ?? Genre;
		Description = description ?? Description;
		return true;
	}


	private static int ValidateTotalCopies(int totalCopies)
	{
		return totalCopies > 0 ? totalCopies : throw new ArgumentException("Invalid total copy value.Please try again");
	}


	public void ChangeAuthor(Author? newAuthor)
	{
		ArgumentNullException.ThrowIfNull(newAuthor);
		if (ReferenceEquals(Author, newAuthor))
			return;

		Author.Books.Remove(this);
		Author = newAuthor;
		newAuthor.Books.Add(this);
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