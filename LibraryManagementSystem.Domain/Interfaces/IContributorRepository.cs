using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IContributorRepository<T> where T : Contributor
{
	void Add(T entity);
	T? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<T> GetAll(EntityFilter filter);
	bool Exists(Expression<Func<T, bool>> predicate, Guid? excludeId = null);
	void Remove(T entity);
	void Update(T entity);
}