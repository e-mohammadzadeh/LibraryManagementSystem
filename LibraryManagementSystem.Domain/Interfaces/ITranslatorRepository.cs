using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository
{
	void Add(Translator translator);
	Translator? FindById(Guid id, EntityFilter filter);
	Translator? FindByName(string firstName, string lastName, EntityFilter filter);
	IReadOnlyList<Translator> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId = null);
	bool ExistsByEmail(Email email, Guid? excludeId = null);
	bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId = null);
	void Remove(Translator translator);
	IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector);
	void Update(Translator translator, Guid? updatedBy);
}