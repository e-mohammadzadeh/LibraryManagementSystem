using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository : IPersonRoleRepository<Translator>
{
	void Update(Translator translator);
	Translator? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.Active);
}