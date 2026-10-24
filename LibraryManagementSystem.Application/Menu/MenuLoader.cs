using System.Text.Json;

namespace LibraryManagementSystem.Application.Menu;

public static class MenuLoader
{
	public static MenuDefinition Load()
	{
		var filePath = Path.Combine(AppContext.BaseDirectory, "Configuration", "Menu", "menu.json");

		if (!File.Exists(filePath))
			throw new FileNotFoundException("Menu configuration file was not found.", filePath);


		var json = File.ReadAllText(filePath);
		return JsonSerializer.Deserialize<MenuDefinition>(
			       json,
			       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
		       ?? throw new InvalidOperationException("The menu configuration is empty or invalid.");
	}
}