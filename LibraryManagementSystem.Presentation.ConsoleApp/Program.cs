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
		//var loanRepo = new InMemoryLoanRepository();

		var userService = new UserManagementService(authorRepo, userRepo);
		var bookService = new BookManagementService(bookRepo, authorRepo);
		//var loanService = new LoanManagementService(loanRepo, bookRepo, UserRepo);


		MainMenu.MainMenuController(userService, bookService);
	}
}