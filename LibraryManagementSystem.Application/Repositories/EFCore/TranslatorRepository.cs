using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

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


	protected override Exception CreateNotFoundException(Guid id) => new TranslatorNotFoundException(id);
}