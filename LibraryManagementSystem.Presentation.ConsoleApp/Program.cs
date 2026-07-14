using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Repositories.InMemory;
using LibraryManagementSystem.Presentation.ConsoleApp.Menus;

namespace LibraryManagementSystem.Presentation.ConsoleApp;

public static class Program
{
	private static void Main()
	{
		var userRepo = new InMemoryUserRepository();
		var authorRepo = new InMemoryAuthorRepository();
		var bookRepo = new InMemoryBookRepository();

		var userService = new UserManagementService(authorRepo, userRepo);
		var bookService = new BookManagementService(bookRepo, userService);


		MainMenu.MainMenuController(userService, bookService);
	}
}