using System.Linq.Expressions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class PersonRepository : IPersonRepository
{
	private readonly LibraryDbContext Context;


	public PersonRepository(LibraryDbContext context) => Context = context;


	public bool Exists(Expression<Func<Person, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		return Context.Set<Person>()
			.AsNoTracking()
			.Where(p => !p.IsRemoved)
			.Where(predicate)
			.Any(p => excludeId == null || p.Id != excludeId);
	}
}