using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
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
		try
		{
			// ── Repositories ──────────────────────────
			var userRepo = new InMemoryUserRepository();
			var authorRepo = new InMemoryAuthorRepository();
			var translatorRepo = new InMemoryTranslatorRepository();
			var bookRepo = new InMemoryBookRepository();
			var roleRepo = new InMemoryRoleRepository();
			var loanRepo = new InMemoryLoanRepository();
			var fineRepo = new InMemoryFineRepository();


			// ── Infrastructure Services ───────────────
			var passwordHasher = new PasswordHasher();
			ICurrentUserSession currentUserSession = new CurrentUserSession();


			Console.Clear();
			Console.Title = "Library Management System";
			// Seed data for development/testing
			DataSeeder.Seed(authorRepo, translatorRepo, bookRepo, userRepo, loanRepo, roleRepo, fineRepo, passwordHasher);


			// ── Application Services ──────────────────
			var authorService = new AuthorManagementService(authorRepo);
			var translatorService = new TranslatorManagementService(translatorRepo);
			var uars = new UserAutoRemovalService(userRepo, loanRepo, fineRepo);
			IFineManagementService fineService = new FineManagementService(fineRepo, loanRepo, userRepo, uars);
			var loanService = new LoanManagementService(loanRepo, userRepo, bookRepo, fineRepo, fineService);
			var userService = new UserManagementService(userRepo, roleRepo, loanRepo, fineRepo, passwordHasher);
			var bookService = new BookManagementService(authorRepo, translatorRepo, bookRepo, loanRepo);
			var authService = new AuthenticationService(userRepo, passwordHasher, currentUserSession);
			var statisticsService = new LibraryStatisticsService(bookRepo, authorRepo, translatorRepo, userRepo, loanRepo);

			while (true)
			{
				var loggedInUser = LoginMenu.ShowLogin(authService);
				if (loggedInUser is null)
					return;

				var result = MainMenu.MainMenuController(authorService, translatorService, userService, bookService, loanService,
					fineService, authService, currentUserSession, statisticsService);

				if (result == MainMenuResult.Exit)
					return;
			}
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n========================================");
			Console.WriteLine("FATAL ERROR DURING STARTUP OR EXECUTION:");
			Console.WriteLine(ex.Message);
			Console.WriteLine("========================================\n");
			Console.ResetColor();

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
		}
	}
}