using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryTranslatorRepository : InMemoryContributorRepository<Translator>, ITranslatorRepository
{
	protected override Exception CreateNotFoundException(Guid id) => new TranslatorNotFoundException(id);
}