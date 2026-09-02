using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Repositories.InMemory;
using LibraryManagementSystem.Infrastructure.Security;
using LibraryManagementSystem.Infrastructure.Seeders;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Menus;
using System.Text;

namespace LibraryManagementSystem.Presentation.ConsoleApp;

public static class Program
{
	private static void Main()
	{
		Console.OutputEncoding = Encoding.UTF8;

		try
		{
			// ── Repositories ──────────────────────────
			var userRepo = new InMemoryUserRepository();
			var authorRepo = new InMemoryAuthorRepository();
			var translatorRepo = new InMemoryTranslatorRepository();
			var bookRepo = new InMemoryBookRepository();
			var roleRepo = new InMemoryRoleRepository();
			var loanRepo = new InMemoryLoanRepository();
			var loanHistoryRepo = new InMemoryLoanHistory();
			var fineRepo = new InMemoryFineRepository();
			var fineHistoryRepo = new InMemoryFineHistory();


			// ── Infrastructure Services ───────────────
			var passwordHasher = new PasswordHasher();
			ICurrentUserSession currentUserSession = new CurrentUserSession();
			IAuthorizationService authorization = new AuthorizationService(currentUserSession, loanRepo);

			Console.Title = "Library Management System";
			// Seed data for development/testing
			DataSeeder.Seed(authorRepo, translatorRepo, bookRepo, userRepo, loanRepo, roleRepo, fineRepo,
				passwordHasher);


			// ── Application Interfaces ──────────────────
			IUserAutoRemovalService userAutoRemovalService = new UserAutoRemovalService(userRepo, loanRepo, fineRepo);
			ILoanHistoryManagementService loanHistoryService = new LoanHistoryManagementService(loanHistoryRepo);
			IFineHistoryManagementService fineHistoryService = new FineHistoryManagementService(fineHistoryRepo);
			IFineManagementService fineService = new FineManagementService(fineRepo, loanRepo, userRepo,
				userAutoRemovalService, authorization, loanHistoryService, fineHistoryService);


			// ── Application Services ──────────────────
			var authorService = new AuthorManagementService(authorRepo, authorization);
			var translatorService = new TranslatorManagementService(translatorRepo, authorization);
			var loanService = new LoanManagementService(loanRepo, userRepo, bookRepo, fineService,
				userAutoRemovalService, authorization, loanHistoryService);
			var loanHistory = new LoanHistoryManagementService(loanHistoryRepo);
			var fineHistory = new FineHistoryManagementService(fineHistoryRepo);
			var userService =
				new UserManagementService(userRepo, roleRepo, loanRepo, fineRepo, passwordHasher, authorization);
			var bookService = new BookManagementService(authorRepo, translatorRepo, bookRepo, loanRepo);
			var authService = new AuthenticationService(userRepo, passwordHasher, currentUserSession);
			var statisticsService =
				new LibraryStatisticsService(bookRepo, authorRepo, translatorRepo, userRepo, loanRepo);

			while (true)
			{
				Console.Clear();
				var loggedInUser = LoginMenu.ShowLogin(authService);
				if (loggedInUser is null) return;

				var result = MainMenu.MainMenuController(authorService, translatorService, userService, bookService,
					loanService, fineService, authService, currentUserSession, authorization, statisticsService,
					loanHistory, fineHistory);

				if (result == MainMenuResult.Exit) return;
				ConsoleHelper.Pause();
			}
		}
		catch (Exception ex)
		{
			ConsoleHelper.ShowError("\n" + new string('=', 50) + "\nFATAL ERROR DURING STARTUP OR EXECUTION:\n" +
			                        ex.Message + "\n" + new string('=', 50));
			ConsoleHelper.Pause();
		}
	}
}