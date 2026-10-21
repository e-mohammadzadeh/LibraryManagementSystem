using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? GetById(Guid id, EntityFilter filter);
	User? GetByName(string firstName, string lastName, EntityFilter filter);
	User? GetByEmail(Email email, EntityFilter filter);
	IReadOnlyList<User> GetAll(EntityFilter filter);
	bool Exists<T>(T value, Func<User, T> selector, Guid? excludeId = null);
	void Remove(User user);
	void Update(User user);
}