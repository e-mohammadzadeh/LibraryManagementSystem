using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository : IPersonRoleRepository<Author>
{
	void Update(Author author);
	Author? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.Active);
}