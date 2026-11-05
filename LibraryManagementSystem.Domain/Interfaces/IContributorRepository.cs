using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IContributorRepository<T> where T : Contributor
{
	void Add(T entity);
	T? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<T> GetAll(EntityFilter filter);
	bool Exists<TValue>(TValue value, Func<T, TValue> selector, Guid? excludeId = null);
	void Remove(T entity);
	void Update(T entity);
}