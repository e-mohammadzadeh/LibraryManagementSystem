using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public abstract class ContributorRepository<T> : PersonRoleRepository<T>, IContributorRepository<T> where T : Contributor
{
	protected ContributorRepository(LibraryDbContext context) : base(context) { }


	public void Update(T contributor)
	{
		ArgumentNullException.ThrowIfNull(contributor);

		var tracked = GetTrackedById(contributor.Id);

		tracked.DeathDate = contributor.DeathDate;
		tracked.Biography = contributor.Biography;

		SaveRoleUpdate(tracked, contributor);
	}
}