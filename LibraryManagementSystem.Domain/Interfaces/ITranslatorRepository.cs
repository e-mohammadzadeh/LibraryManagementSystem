using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository : IPersonRoleRepository<Translator>
{
	void Update(Translator translator);
}