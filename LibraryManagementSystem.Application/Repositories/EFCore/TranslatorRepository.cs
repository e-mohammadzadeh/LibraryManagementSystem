using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class TranslatorRepository : PersonRoleRepository<Translator>, ITranslatorRepository
{
	public TranslatorRepository(LibraryDbContext context) : base(context) { }


	public void Update(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

		var tracked = GetTrackedById(translator.Id);

		tracked.DeathDate = translator.DeathDate;
		tracked.Biography = translator.Biography;

		SaveRoleUpdate(tracked, translator);
	}


	public Translator? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.Active)
	{
		var query = Context.Set<Translator>()
			.Include(t => t.Person)
			.AsQueryable();

		query = filter switch
		{
			EntityFilter.Active => query.Where(t => !t.IsRemoved),
			EntityFilter.Removed => query.Where(t => t.IsRemoved),
			EntityFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};

		return query.FirstOrDefault(t => t.PersonId == personId);
	}


	protected override Exception CreateNotFoundException(Guid id) => new TranslatorNotFoundException(id);
}