namespace LibraryManagementSystem.Application.DTOs.Library;

public class LibraryStatisticsDto
{
	public int TotalBooks { get; init; }
	public int TotalAuthors { get; init; }
	public int TotalUsers { get; init; }
	public int TotalActiveLoans { get; init; }
	public int TotalRoles { get; init; }
}