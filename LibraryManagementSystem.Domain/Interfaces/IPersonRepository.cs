using System.Linq.Expressions;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRepository
{
	bool ExistsActive(Expression<Func<Person, bool>> predicate, Guid? excludeId = null);
	bool ExistsIncludingRemoved(Expression<Func<Person, bool>> predicate, Guid? excludeId = null);
}