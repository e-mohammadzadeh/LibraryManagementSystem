using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IContributorRepository<T> : IPersonRoleRepository<T> where T : Contributor 
{
	void Update(T contributor);
}