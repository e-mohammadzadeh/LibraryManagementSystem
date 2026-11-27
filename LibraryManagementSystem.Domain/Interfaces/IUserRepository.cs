using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository : IPersonRoleRepository<User>
{
	void Update(User user);
	override User? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
}