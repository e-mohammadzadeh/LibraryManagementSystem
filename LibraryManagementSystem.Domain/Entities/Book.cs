using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

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
	public string Title { get; set; }
	public string InternationalStandardBookNumber { get; set; }
	public List<BookAuthor> BookAuthors { get; set; }
	public List<BookTranslator> BookTranslators { get; set; }
	public DateOnly PublishDate { get; set; }
	public Genre Genre { get; set; }
	public string Publisher { get; set; }
	public int TotalCopies { get; set; }
	public int AvailableCopies { get; set; }
	public string? Description { get; set; }
	public DateTime CreatedAt { get; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsRemoved { get; set; }


	private static int ValidateTotalCopies(int totalCopies)
	{
		return totalCopies > 0 ? totalCopies : throw new ArgumentException("Invalid total copy value.Please try again");
	}
}