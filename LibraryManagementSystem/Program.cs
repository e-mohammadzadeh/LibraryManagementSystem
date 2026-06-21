using LibraryManagementSystem.Presentation;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem;

public static class Program
{
	private static void Main()
	{
		var bookManagementService = new BookManagementService();
		var userManagementService = new UserManagementService();
		MainMenu.MainMenuController(userManagementService, bookManagementService);
	}
}