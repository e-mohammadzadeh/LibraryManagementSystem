using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IContributorRepository<T> where T : Person
{
	void Add(T entity);
	T? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<T> GetAll(EntityFilter filter);
	void Remove(T entity);
	void Update(T entity);
}