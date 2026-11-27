using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class PersonRepository : IPersonRepository
{
	private readonly LibraryDbContext _context;


	public PersonRepository(LibraryDbContext context) => _context = context;


	public void Add(Person person)
	{
		ArgumentNullException.ThrowIfNull(person);

		person.Id = Guid.CreateVersion7();
		person.CreatedAt = DateTime.UtcNow;

		_context.Set<Person>().Add(person);
		_context.SaveChanges();
		_context.ChangeTracker.Clear();
	}


	public Person? GetById(Guid id)
	{
		return _context.Set<Person>()
			.AsNoTracking()
			.Include(p => p.Author)
			.Include(p => p.Translator)
			.Include(p => p.User)
			.FirstOrDefault(p => p.Id == id);
	}


	public Person? GetByNationalCode(string nationalCode, EntityFilter filter = EntityFilter.All)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(nationalCode);

		IQueryable<Person> query = _context.Set<Person>()
			.AsNoTracking()
			.Include(p => p.Author)
			.Include(p => p.Translator)
			.Include(p => p.User);

		query = ApplyPersonFilter(query, filter);
		return query.FirstOrDefault(p => p.NationalCode == nationalCode);
	}


	public bool ExistsActive(Expression<Func<Person, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		var query = ApplyPersonFilter(
			_context.Set<Person>().AsNoTracking(),
			EntityFilter.Active);

		query = query.Where(predicate);

		if (excludeId.HasValue) query = query.Where(p => p.Id != excludeId.Value);
		return query.Any();
	}


	public bool ExistsIncludingRemoved(Expression<Func<Person, bool>> predicate, Guid? excludeId = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		var query = _context.Set<Person>()
			.AsNoTracking()
			.Where(predicate);

		if (excludeId.HasValue) query = query.Where(p => p.Id != excludeId.Value);

		return query.Any();
	}


	public void Update(Person person)
	{
		ArgumentNullException.ThrowIfNull(person);

		var tracked = _context.Set<Person>()
			.Find(person.Id) ?? throw new PersonNotFoundException(person.Id);

		tracked.FirstName = person.FirstName;
		tracked.LastName = person.LastName;
		tracked.NationalCode = person.NationalCode;
		tracked.Email = person.Email;
		tracked.PhoneNumber = person.PhoneNumber;
		tracked.BirthDate = person.BirthDate;

		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = person.UpdatedByUserId;

		_context.SaveChanges();
	}


	private static IQueryable<Person> ApplyPersonFilter(IQueryable<Person> query, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => query.Where(p =>
				(p.Author != null && !p.Author.IsRemoved) ||
				(p.Translator != null && !p.Translator.IsRemoved) ||
				(p.User != null && !p.User.IsRemoved)),

			EntityFilter.Removed => query.Where(p =>
				(p.Author == null || p.Author.IsRemoved) &&
				(p.Translator == null || p.Translator.IsRemoved) &&
				(p.User == null || p.User.IsRemoved)),

			EntityFilter.All => query,

			_ => throw new ArgumentOutOfRangeException(
				nameof(filter), filter, null)
		};
	}
}