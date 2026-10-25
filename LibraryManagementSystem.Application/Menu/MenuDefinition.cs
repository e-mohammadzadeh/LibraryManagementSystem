namespace LibraryManagementSystem.Application.Menu;

public sealed class MenuDefinition
{
	public string Title { get; init; } = string.Empty;
	public List<MenuItem> Items { get; init; } = [];
}