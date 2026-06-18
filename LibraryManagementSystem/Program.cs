using LibraryManagementSystem.Presentation;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem;

public static class Program
{
	private static void Main()
	{
		BookManagementService bookManagementService = new BookManagementService();
		MainMenu.MainMenuController(bookManagementService);
	}
}

