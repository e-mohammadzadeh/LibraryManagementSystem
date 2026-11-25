using System.Linq.Expressions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRepository
{
	void Add(Person person);
	Person? GetById(Guid id);
	Person? GetByNationalCode(string nationalCode, EntityFilter filter = EntityFilter.All);
	bool ExistsActive(Expression<Func<Person, bool>> predicate, Guid? excludeId = null);
	bool ExistsIncludingRemoved(Expression<Func<Person, bool>> predicate, Guid? excludeId = null);
	void Update(Person person);
}