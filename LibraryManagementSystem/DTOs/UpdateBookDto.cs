using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem.DTOs;

public class UpdateBookDto
{
	public string? BookName { get; set; }
	public string? ISBN { get; set; }
	public Author? Author { get; set; }
	public DateOnly? PublishDate { get; set; }
	public Genre? Genre { get; set; }
	public int? TotalCopies { get; set; }
	public string? Description { get; set; }
}