using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.Domain.Persistence;

public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
	public LibraryDbContext CreateDbContext(string[] args)
	{
		var configurationPath = FindConfigurationDirectory();

		var configuration = new ConfigurationBuilder()
			.SetBasePath(configurationPath)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
			.AddEnvironmentVariables()
			.Build();

		var connectionString = configuration.GetConnectionString("LibraryDb") ??
		                       throw new InvalidOperationException("Connection string 'LibraryDb' was not found.");

		var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();

		optionsBuilder.UseSqlServer(connectionString);

		return new LibraryDbContext(optionsBuilder.Options);
	}



	private static string FindConfigurationDirectory()
	{
		var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

		while (currentDirectory != null)
		{
			var candidates = new[]
			{
				Path.Combine(currentDirectory.FullName, "Configuration"),
				Path.Combine(currentDirectory.FullName, "LibraryManagementSystem.Presentation.ConsoleApp",
					"Configuration")
			};

			foreach (var candidate in candidates)
				if (File.Exists(Path.Combine(candidate, "appsettings.json")))
					return candidate;
			currentDirectory = currentDirectory.Parent;
		}

		throw new DirectoryNotFoundException("Could not locate Configuration/appsettings.json.");
	}
}