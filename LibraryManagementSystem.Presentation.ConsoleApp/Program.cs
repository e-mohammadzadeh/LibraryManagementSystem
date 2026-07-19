using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Repositories.InMemory;
using LibraryManagementSystem.Infrastructure.Seeders;
using LibraryManagementSystem.Presentation.ConsoleApp.Menus;

namespace LibraryManagementSystem.Presentation.ConsoleApp;

public static class Program
{
	private static void Main()
	{
		var userRepo = new InMemoryUserRepository();
		var authorRepo = new InMemoryAuthorRepository();
		var bookRepo = new InMemoryBookRepository();
		var roleRepo = new InMemoryRoleRepository();
		var loanRepo = new InMemoryLoanRepository();

		// Seed data for development/testing
		DataSeeder.Seed(authorRepo, bookRepo, userRepo, loanRepo, roleRepo);

		var authorService = new AuthorManagementService(authorRepo);
		var userService = new UserManagementService(userRepo, roleRepo);
		var bookService = new BookManagementService(bookRepo, authorRepo);
		var loanService = new LoanManagementService(loanRepo, userRepo, bookRepo);

		var statisticsService = new LibraryStatisticsService(bookRepo, authorRepo, userRepo, loanRepo);


		MainMenu.MainMenuController(authorService, userService, bookService, loanService, statisticsService);
	}
}