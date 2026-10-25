namespace LibraryManagementSystem.Application.Menu;

public sealed class MenuService
{
	private readonly MenuDefinition _menu;


	public MenuService(MenuDefinition menu)
	{
		ArgumentNullException.ThrowIfNull(menu);
		_menu = menu;
	}


	public IReadOnlyList<MenuOptionDto> GetChildren(string id)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id);

		var item = FindItem(_menu.Items, id);

		if (item is null) return [];

		return [.. item.Children.Select(child => new MenuOptionDto(child.Id, child.Label))];
	}


	private static MenuItem? FindItem(IEnumerable<MenuItem> items, string id)
	{
		foreach (var item in items)
		{
			if (string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase)) return item;
			var found = FindItem(item.Children, id);
			if (found is not null) return found;
		}

		return null;
	}
}