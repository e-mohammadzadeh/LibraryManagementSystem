using LibraryManagementSystem.Infrastructure.Enums.Filters;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRoleRepository<T> where T : class, IPersonRoleEntity
{
	void Add(T entity);
	T? GetById(Guid id, EntityFilter filter = EntityFilter.Active);
	T? GetByPersonId(Guid personId, EntityFilter filter = EntityFilter.All);
	IReadOnlyList<T> GetAll(EntityFilter filter = EntityFilter.Active);
	void Remove(T entity);
	void Restore(T entity, Guid? updatedByUserId = null);
	bool Exists(Expression<Func<T, bool>> predicate, Guid? excludeId = null);
	bool ExistsIncludingRemoved(Expression<Func<T, bool>> predicate, Guid? excludeId = null);
}