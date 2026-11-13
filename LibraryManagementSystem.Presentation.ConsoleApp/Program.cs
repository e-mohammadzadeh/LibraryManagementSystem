using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Security;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Menus;
using System.Text;
using LibraryManagementSystem.Application.Menu;
using LibraryManagementSystem.Application.Repositories.EFCore;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace LibraryManagementSystem.Presentation.ConsoleApp;

public static class Program
{
	private static void Main()
	{
		Console.OutputEncoding = Encoding.UTF8;

		try
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(AppContext.BaseDirectory, "Configuration"))
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			var connectionString = config.GetConnectionString("LibraryDb")
			                       ?? throw new InvalidOperationException(
				                       "Connection string 'LibraryDb' was not found.");

			using var provider = BuildServiceProvider(connectionString);
			using var scope = provider.CreateScope();
			var sp = scope.ServiceProvider;

			// ── Database ─────────────────────────────────
			sp.GetRequiredService<LibraryDbContext>().Database.Migrate();
			Console.WriteLine("Database ready!");


			// ── Resolve what the menus need ───────────
			var currentUserSession = sp.GetRequiredService<ICurrentUserSession>();
			var authorization = sp.GetRequiredService<IAuthorizationService>();
			var authService = sp.GetRequiredService<AuthenticationService>();
			var authorService = sp.GetRequiredService<AuthorManagementService>();
			var translatorService = sp.GetRequiredService<TranslatorManagementService>();
			var userService = sp.GetRequiredService<UserManagementService>();
			var bookService = sp.GetRequiredService<BookManagementService>();
			var loanService = sp.GetRequiredService<LoanManagementService>();
			var fineService = sp.GetRequiredService<IFineManagementService>();
			var statisticsService = sp.GetRequiredService<LibraryStatisticsService>();
			var auditLogService = sp.GetRequiredService<IAuditLogManagementService>();
			var roleService = sp.GetRequiredService<RoleManagementService>();
			var menuService = sp.GetRequiredService<IMenuService>();

			Console.Title = "Library Management System";

			while (true)
			{
				Console.Clear();
				var loggedInUser = LoginMenu.ShowLoginMenu(authService);
				ConsoleHelper.Pause();
				if (loggedInUser is null) return;

				var result = MainMenu.MainMenuController(authorService, translatorService, userService, bookService,
					loanService, fineService, authService, currentUserSession, authorization, statisticsService,
					auditLogService, roleService, menuService);

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


	private static ServiceProvider BuildServiceProvider(string connectionString)
	{
		var services = new ServiceCollection();

		// ── Database ─────────────────────────────────
		services.AddDbContext<LibraryDbContext>(o => o.UseSqlServer(connectionString));

		// ── Singletons (no scoped dependencies) ───────────────────────────────
		services.AddSingleton(MenuLoader.Load());
		services.AddSingleton<IPasswordHasher, PasswordHasher>();

		// ── Repositories ──────────────────────────────────────────────────────
		services.AddScoped<IRoleRepository, RoleRepository>();
		services.AddScoped<IAuthorRepository, AuthorRepository>();
		services.AddScoped<ITranslatorRepository, TranslatorRepository>();
		services.AddScoped<IBookRepository, BookRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ILoanRepository, LoanRepository>();
		services.AddScoped<IFineRepository, FineRepository>();
		services.AddScoped<IAuditLogRepository, AuditLogRepository>();

		// ── Infrastructure services ─────────────────────────────────────────────
		services.AddScoped<ICurrentUserSession, CurrentUserSession>();
		services.AddScoped<IAuthorizationService, AuthorizationService>();

		// ── Application services (interface + concrete share one instance) ──────
		services.AddScoped<AuditLogManagementService>();
		services.AddScoped<IAuditLogManagementService>(sp => sp.GetRequiredService<AuditLogManagementService>());

		services.AddScoped<UserAutoRemovalService>();
		services.AddScoped<IUserAutoRemovalService>(sp => sp.GetRequiredService<UserAutoRemovalService>());

		services.AddScoped<BookManagementService>();
		services.AddScoped<IBookManagementService>(sp => sp.GetRequiredService<BookManagementService>());

		services.AddScoped<FineManagementService>();
		services.AddScoped<IFineManagementService>(sp => sp.GetRequiredService<FineManagementService>());

		services.AddScoped<UserManagementService>();
		services.AddScoped<IUserManagementService>(sp => sp.GetRequiredService<UserManagementService>());

		services.AddScoped<MenuService>();
		services.AddScoped<IMenuService>(sp => sp.GetRequiredService<MenuService>());

		services.AddScoped<AuthorManagementService>();
		services.AddScoped<TranslatorManagementService>();
		services.AddScoped<LoanManagementService>();
		services.AddScoped<AuthenticationService>();
		services.AddScoped<LibraryStatisticsService>();
		services.AddScoped<RoleManagementService>();

		return services.BuildServiceProvider(new ServiceProviderOptions
		{
			ValidateScopes = true,
			ValidateOnBuild = true
		});
	}
}