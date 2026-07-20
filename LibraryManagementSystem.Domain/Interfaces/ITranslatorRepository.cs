using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository
{
	void Add(Translator translator);
	Translator? FindById(int id);
	Translator? FindByName(string firstName, string lastName);
	IReadOnlyList<Translator> GetAll();
	bool ExistsByName(string firstName, string lastName, int excludeId = -1);
	bool ExistsByNationalCode(string nationalCode, int excludeId = -1);
	bool ExistsByEmail(string email, int excludeId = -1);
	bool ExistsByPhoneNumber(string phoneNumber, int excludeId = -1);
	void Remove(Translator translator);
	IReadOnlyList<Translator> Search(string searchItem, Func<Translator, string?> selector);
	int Count();
}