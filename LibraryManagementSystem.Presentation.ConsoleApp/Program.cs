using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Repositories.InMemory;
using LibraryManagementSystem.Infrastructure.Security;
using LibraryManagementSystem.Infrastructure.Seeders;
using LibraryManagementSystem.Presentation.ConsoleApp.Menus;

namespace LibraryManagementSystem.Presentation.ConsoleApp;

public static class Program
{
	private static void Main()
	{
		var userRepo = new InMemoryUserRepository();
		var authorRepo = new InMemoryAuthorRepository();
		var translatorRepo = new InMemoryTranslatorRepository();
		var bookRepo = new InMemoryBookRepository();
		var roleRepo = new InMemoryRoleRepository();
		var loanRepo = new InMemoryLoanRepository();
		var fineRepo = new InMemoryFineRepository();
		var passwordRepo = new PasswordHasher();
		ICurrentUserSession currentUserSession = new CurrentUserSession();

		Console.Clear();
		Console.Title = "Library Management System";

		// Seed data for development/testing
		DataSeeder.Seed(authorRepo, translatorRepo, bookRepo, userRepo, loanRepo, roleRepo, fineRepo);

		var authorService = new AuthorManagementService(authorRepo);
		var translatorService = new TranslatorManagementService(translatorRepo);
		var userService = new UserManagementService(userRepo, roleRepo, loanRepo, passwordRepo);
		var bookService = new BookManagementService(authorRepo, translatorRepo, bookRepo, loanRepo);
		var uars = new UserAutoRemovalService(userRepo, loanRepo, fineRepo);
		var fineService = new FineManagementService(fineRepo, loanRepo, userRepo, uars);
		var loanService = new LoanManagementService(loanRepo, userRepo, bookRepo, fineRepo, fineService);
		var authService = new AuthenticationService(userRepo, passwordRepo, currentUserSession);
		var statisticsService = new LibraryStatisticsService(bookRepo, authorRepo, translatorRepo, userRepo, loanRepo);

		while (true)
		{
			var loggedInUser = LoginMenu.ShowLogin(authService);
			if (loggedInUser is null) return;

			MainMenu.MainMenuController(authorService, translatorService, userService, bookService, loanService,
				fineService, authService, currentUserSession, statisticsService);
		}
	}
}