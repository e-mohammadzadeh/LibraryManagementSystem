namespace LibraryManagementSystem.Application.Menu;

public sealed class MenuItem
{
	public string Id { get; init; } = string.Empty;
	public string Label { get; init; } = string.Empty;
	public List<MenuItem> Children { get; init; } = [];
}