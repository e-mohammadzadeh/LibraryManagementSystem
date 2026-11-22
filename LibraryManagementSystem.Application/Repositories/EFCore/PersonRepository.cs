using System.Linq.Expressions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class PersonRepository : IPersonRepository
{
	private readonly LibraryDbContext _context;


	public PersonRepository(LibraryDbContext context) => _context = context;


	public bool Exists(Expression<Func<Person, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		return _context.Set<Person>()
			.AsNoTracking()
			.Where(p => !p.IsRemoved)
			.Where(predicate)
			.Any(p => excludeId == null || p.Id != excludeId);
	}


	public bool ExistsIncludingRemoved(Expression<Func<Person, bool>> predicate, Guid? excludeId = null) 
	{
		ArgumentNullException.ThrowIfNull(predicate);

		var query = _context.Set<Person>()
			.AsNoTracking()
			.Where(predicate);

		if (excludeId.HasValue)
			query = query.Where(p => p.Id != excludeId.Value);

		return query.Any();
	}
}