using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRoleRepository<T> where T : class, IPersonRoleEntity
{
	void Add(T entity);
	T? GetById(Guid Id, EntityFilter filter = EntityFilter.Active);
	IReadOnlyList<T> GetAll(EntityFilter filter = EntityFilter.Active);
	void Remove(T entity);
}