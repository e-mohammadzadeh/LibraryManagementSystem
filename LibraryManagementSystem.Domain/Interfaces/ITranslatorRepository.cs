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
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(Email email, Guid? excludeId);
	bool ExistsByPhoneNumber(PhoneNumber phoneNumber, Guid? excludeId);
	void Remove(Translator translator);
	IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector);
	void Update(Translator translator, Guid? updatedBy);
}