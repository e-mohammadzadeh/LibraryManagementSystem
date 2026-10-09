using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryTranslatorRepository : ITranslatorRepository
{
	private readonly List<Translator> _translators = [];


	public void Add(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

		translator.Id = Guid.CreateVersion7();
		translator.CreatedAt = DateTime.UtcNow;
		translator.IsRemoved = false;
		_translators.Add(translator);
	}


	public Translator? FindById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_translators, filter).FirstOrDefault(t => t.Id == id);
	}


	public Translator? FindByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_translators, filter).FirstOrDefault(t =>
			!t.IsRemoved &&
			t.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			t.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Translator> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_translators, filter)];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId)
	{
		return _translators.Any(t =>
			!t.IsRemoved &&
			t.NationalCode.Equals(nationalCode, StringComparison.OrdinalIgnoreCase) &&
			(excludeId is null || t.Id != excludeId));
	}


	public bool ExistsByEmail(Email email, Guid? excludeId)
	{
		return _translators.Any(t =>
			!t.IsRemoved &&
			t.Email.Equals(email) &&
			(excludeId is null || t.Id != excludeId));
	}


	public bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId)
	{
		return _translators.Any(t =>
			!t.IsRemoved &&
			t.PhoneNumber.Equals(phoneNumber) &&
			(excludeId is null || t.Id != excludeId));
	}


	public void Remove(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);
		if (translator.IsRemoved) return;
		translator.IsRemoved = true;
		translator.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		return
		[
			.. _translators
				.Where(t => !t.IsRemoved)
				.Where(t =>
			{
				var value = selector(t);
				return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public void Update(Translator translator, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(translator);

		var tracked = _translators.FirstOrDefault(a => a.Id == translator.Id) ?? throw new TranslatorNotFoundException(translator.Id);

		tracked.FirstName = translator.FirstName;
		tracked.LastName = translator.LastName;
		tracked.NationalCode = translator.NationalCode;
		tracked.Email = translator.Email;
		tracked.PhoneNumber = translator.PhoneNumber;
		tracked.BirthDate = translator.BirthDate;
		tracked.Biography = translator.Biography;
		tracked.UpdatedByUserId = updatedBy;
		tracked.UpdatedAt = DateTime.UtcNow;
	}


	// ---------- Private helper ----------
	private static IEnumerable<Translator> ApplyFilter(IEnumerable<Translator> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(a => !a.IsRemoved),
			EntityFilter.Removed => source.Where(a => a.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}