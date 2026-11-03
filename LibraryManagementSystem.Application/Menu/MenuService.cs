using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Infrastructure.DTOs;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Menu;

public sealed class MenuService : IMenuService
{
	private readonly MenuDefinition _menu;
	private readonly IAuthorizationService _authorization;


	public MenuService(MenuDefinition menu, IAuthorizationService authorization)
	{
		ArgumentNullException.ThrowIfNull(menu);
		_menu = menu;
		_authorization = authorization;
	}


	public IReadOnlyList<MenuOptionDto> GetChildren(string id)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id);

		var item = FindItem(_menu.Items, id);

		if (item is null) return [];

		return [.. item.Children.Where(CanDisplay).Select(child => new MenuOptionDto(child.Id, child.Label))];
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


	private bool CanDisplay(MenuItem item)
	{
		if (item.Permissions.Count == 0)
			return true;

		foreach (var permissionName in item.Permissions)
		{
			if (!Enum.TryParse<Permission>(
				    permissionName, true, out var permission))
			{
				throw new InvalidOperationException(
					$"Unknown permission '{permissionName}' in menu configuration.");
			}

			if (_authorization.HasPermission(permission))
				return true;
		}

		return false;
	}
}