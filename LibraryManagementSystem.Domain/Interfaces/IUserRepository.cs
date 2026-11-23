using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository : IPersonRoleRepository<User>
{
	void Update(User user);
}