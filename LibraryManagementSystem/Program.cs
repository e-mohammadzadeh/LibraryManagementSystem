using LibraryManagementSystem.Presentation;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem;

public static class Program
{
	private static void Main()
	{
		List<int> grades = new() { 1, 2, 3, 4, 5 };

		var bookManagementService = new BookManagementService();
		var userManagementService = new UserManagementService();
		MainMenu.MainMenuController(userManagementService, bookManagementService);
	}
}