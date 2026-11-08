using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Domain.Persistence;

public class LibraryDbContext : DbContext
{
	public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
	public DbSet<Author> Authors => Set<Author>();
	public DbSet<Translator> Translators => Set<Translator>();
	public DbSet<Book> Books => Set<Book>();
	public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
	public DbSet<BookTranslator> BookTranslations => Set<BookTranslator>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Loan> Loans => Set<Loan>();
	public DbSet<Fine> Fines => Set<Fine>();
	public DbSet<Role> Roles => Set<Role>();


	protected override void OnModelCreating(ModelBuilder moderBuilder)
	{
		base.OnModelCreating(moderBuilder);

		moderBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
	}
}