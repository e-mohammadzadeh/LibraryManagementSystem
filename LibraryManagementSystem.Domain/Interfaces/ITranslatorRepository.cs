using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITranslatorRepository
{
	void Add(Translator translator);
	Translator? GetById(Guid id, EntityFilter filter);
	IReadOnlyList<Translator> GetAll(EntityFilter filter);
	bool Exists<T>(T value, Func<Translator, T> selector, Guid? excludeId = null);
	void Remove(Translator translator);
	void Update(Translator translator);
}