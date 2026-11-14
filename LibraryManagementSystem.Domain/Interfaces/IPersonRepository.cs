using System.Linq.Expressions;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRepository
{
	bool Exists(Expression<Func<Person, bool>> predicate, Guid? excludeId = null);
}