using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryContributorRepository<T> : IContributorRepository<T> where T : Contributor
{
	private readonly List<T> _items = [];


	public void Add(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		_items.Add(entity);
	}


	public T? GetById(Guid id, EntityFilter filter = EntityFilter.Active) =>
		ApplyFilter(_items, filter).FirstOrDefault(x => x.Id == id);


	public IReadOnlyList<T> GetAll(EntityFilter filter = EntityFilter.Active) => [.. ApplyFilter(_items, filter)];


	public bool Exists<TValue>(TValue value, Func<T, TValue> selector, Guid? excludeId) =>
		_items.Any(x =>
			!x.IsRemoved &&
			(excludeId is null || x.Id != excludeId) &&
			EqualityComparer<TValue>.Default.Equals(selector(x), value));


	public void Remove(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		if (entity.IsRemoved) return;

		entity.IsRemoved = true;
		entity.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(T entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		var tracked = _items.FirstOrDefault(x => x.Id == entity.Id) ?? throw CreateNotFoundException(entity.Id);

		tracked.FirstName = entity.FirstName;
		tracked.LastName = entity.LastName;
		tracked.NationalCode = entity.NationalCode;
		tracked.Email = entity.Email;
		tracked.PhoneNumber = entity.PhoneNumber;
		tracked.BirthDate = entity.BirthDate;
		tracked.Biography = entity.Biography;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = entity.UpdatedByUserId;
	}


	protected virtual Exception CreateNotFoundException(Guid id) =>
		new InvalidOperationException($"{typeof(T).Name} with Id '{id}' was not found.");


	private static IEnumerable<T> ApplyFilter(IEnumerable<T> source, EntityFilter filter) =>
		filter switch
		{
			EntityFilter.Active => source.Where(x => !x.IsRemoved),
			EntityFilter.Removed => source.Where(x => x.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
}