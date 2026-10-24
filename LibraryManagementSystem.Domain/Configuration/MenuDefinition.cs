namespace LibraryManagementSystem.Domain.Configuration;

public class MenuDefinition
{
	public string Title { get; init; } = string.Empty;
	public List<MenuItem> Items { get; init; } = [];
}