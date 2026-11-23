using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository : IPersonRoleRepository<Author>
{
	void Update(Author author);
}