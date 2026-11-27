using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository : IPersonRoleRepository<User>
{
	void Update(User user);
	User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active);
	User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active);
}