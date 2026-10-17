using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuthorRepository
{
	void Add(Author author);
	Author? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<Author> GetAll(EntityFilter filter);
	bool Exists<T>(T value, Func<Author, T> selector, Guid? excludeId = null);
	void Remove(Author author);
	void Update(Author author);
}