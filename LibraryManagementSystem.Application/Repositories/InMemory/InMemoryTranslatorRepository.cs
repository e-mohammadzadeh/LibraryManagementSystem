using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryTranslatorRepository : ITranslatorRepository
{
	private readonly List<Translator> _translators = [];


	public void Add(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);
		_translators.Add(translator);
		translator.UpdatedAt = DateTime.UtcNow;
	}


	public Translator? FindById(Guid id)
	{
		return _translators.FirstOrDefault(translator => translator.Id == id && !translator.IsRemoved);
	}


	public Translator? FindByName(string firstName, string lastName)
	{
		return _translators.FirstOrDefault(translator =>
			!translator.IsRemoved &&
			translator.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			translator.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Translator> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		var query = _translators.AsEnumerable();
		switch (filter)
		{
			case EntityFilter.Active:
				query = query.Where(a => !a.IsRemoved);
				break;
			case EntityFilter.Removed:
				query = query.Where(a => a.IsRemoved);
				break;
			case EntityFilter.All:
				// No filter – include everyone
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
		}

		return [.. query];
	}


	public bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null)
	{
		return _translators.Any(translator =>
			translator.Id != excludeId &&
			!translator.IsRemoved &&
			translator.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, Guid? excludeId = null)
	{
		return _translators.Any(translator =>
			translator.Id != excludeId &&
			!translator.IsRemoved &&
			translator.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId = null)
	{
		return _translators.Any(translator =>
			translator.Id != excludeId &&
			!translator.IsRemoved &&
			translator.PhoneNumber.Equals(phoneNumber));
	}


	public void Remove(Translator translator)
	{
		if (translator.IsRemoved) return;
		translator.IsRemoved = true;
		translator.UpdatedAt = DateTime.UtcNow;
	}


	public IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		return
		[
			.. _translators.Where(translator =>
			{
				var value = selector(translator);
				return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public void Update(Translator translator, UpdateContributorDto dto)
	{
		translator.FirstName = dto.FirstName ?? translator.FirstName;
		translator.LastName = dto.LastName ?? translator.LastName;
		translator.NationalCode = dto.NationalCode ?? translator.NationalCode;
		translator.Email = dto.Email ?? translator.Email;
		translator.PhoneNumber = dto.PhoneNumber ?? translator.PhoneNumber;
		translator.BirthDate = dto.BirthDate ?? translator.BirthDate;
		translator.Biography = dto.Biography ?? translator.Biography;
		translator.UpdatedAt = DateTime.UtcNow;
	}
}