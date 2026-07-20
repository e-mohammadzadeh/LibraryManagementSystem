using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryTranslatorRepository : ITranslatorRepository
{
	private readonly List<Translator> _translators = [];


	public void Add(Translator translator)
	{
		_translators.Add(translator);
	}


	public Translator? FindById(int id)
	{
		return _translators.FirstOrDefault(translator => translator.Id == id);
	}


	public Translator? FindByName(string firstName, string lastName)
	{
		return _translators.FirstOrDefault(translator =>
			translator.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			translator.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public IReadOnlyList<Translator> GetAll()
	{
		return _translators.AsReadOnly();
	}


	public bool ExistsByName(string firstName, string lastName, int excludeId = -1)
	{
		return _translators.Any(translator =>
			translator.Id != excludeId &&
			translator.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			translator.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByNationalCode(string nationalCode, int excludeId = -1)
	{
		return _translators.Any(translator => translator.Id != excludeId &&
			translator.NationalCode.Equals(nationalCode));
	}


	public bool ExistsByEmail(string email, int excludeId = -1)
	{
		return _translators.Any(translator => translator.Id != excludeId &&
			translator.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1)
	{
		return _translators.Any(translator => translator.Id != excludeId &&
			translator.PhoneNumber.Equals(phoneNumber));
	}


	public void Remove(Translator translator)
	{
		_translators.Remove(translator);
	}


	public IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return [];

		return
		[
			.. _translators.Where(translator =>
			{
				var value = selector(translator);
				return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
			})
		];
	}


	public int Count()
	{
		return _translators.Count;
	}
}