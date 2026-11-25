using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class AuthorRepository : PersonRoleRepository<Author>, IAuthorRepository
{
	public AuthorRepository(LibraryDbContext context) : base(context) { }


	public void Update(Author author)
	{
		ArgumentNullException.ThrowIfNull(author);

		var tracked = GetTrackedById(author.Id);

		tracked.DeathDate = author.DeathDate;
		tracked.Biography = author.Biography;

		SaveRoleUpdate(tracked, author);
	}


	public Author? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.Active)
	{
		var query = Context.Set<Author>()
			.Include(a => a.Person)
			.AsQueryable();

		query = filter switch
		{
			EntityFilter.Active => query.Where(a => !a.IsRemoved),
			EntityFilter.Removed => query.Where(a => a.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return query.FirstOrDefault(a => a.PersonId == personId);
	}


	protected override Exception CreateNotFoundException(Guid id) => new AuthorNotFoundException(id);
}