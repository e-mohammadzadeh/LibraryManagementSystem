namespace LibraryManagementSystem.Domain.Entities;

public abstract class Contributor : PersonProfile
{
	public DateOnly? DeathDate { get; set; }
	public string? Biography { get; set; }
}