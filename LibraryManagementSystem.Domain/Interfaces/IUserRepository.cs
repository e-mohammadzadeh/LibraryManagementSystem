using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
	void Add(User user);
	User? GetById(Guid id, EntityFilter filter);
	User? GetByName(string firstName, string lastName, EntityFilter filter);
	User? GetByEmail(Email email, EntityFilter filter);
	IReadOnlyList<User> GetAll(EntityFilter filter);
	bool HasUserWithRole(Guid roleId); 
	void Remove(User user);
	void Update(User user);
}