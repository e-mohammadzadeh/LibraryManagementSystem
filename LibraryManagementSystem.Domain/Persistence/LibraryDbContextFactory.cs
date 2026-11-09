using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryManagementSystem.Domain.Persistence;

public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
	public LibraryDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
		optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=LibraryManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
		return new LibraryDbContext(optionsBuilder.Options);
	}
}