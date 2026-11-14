namespace LibraryManagementSystem.Domain.Entities;

public abstract class Contributor : Person
{
	public DateOnly? DeathDate { get; set; }
	public string? Biography { get; set; }
}