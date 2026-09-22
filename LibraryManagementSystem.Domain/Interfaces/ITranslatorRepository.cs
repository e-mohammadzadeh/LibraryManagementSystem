using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository
{
	void Add(Translator translator);
	Translator? FindById(Guid id);
	Translator? FindByName(string firstName, string lastName);
	IReadOnlyList<Translator> GetAll(EntityFilter filter);
	bool ExistsByNationalCode(string nationalCode, Guid? excludeId);
	bool ExistsByEmail(string email, Guid? excludeId);
	bool ExistsByPhoneNumber(string phoneNumber, Guid? excludeId);
	void Remove(Translator translator);
	IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector);
	void Update(Translator translator, UpdateContributorDto dto);
}