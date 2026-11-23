using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class AuthorRepository : PersonRoleRepository<Author>, IAuthorRepository
{
	public AuthorRepository(LibraryDbContext context) : base(context)
	{

	}


	public void Update(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		var tracked = GetTrackedById(author.Id);

		tracked.DeathDate = author.DeathDate;
		tracked.Biography = author.Biography;

		SaveRoleUpdate(tracked,author);
	}

	protected override Exception CreateNotFoundException(Guid id) => new AuthorNotFoundException(id);
}