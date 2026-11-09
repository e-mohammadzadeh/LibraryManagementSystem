using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class TranslatorRepository : ContributorRepository<Translator>, ITranslatorRepository
{
	public TranslatorRepository(LibraryDbContext context) : base(context) { }


	protected override Exception CreateNotFoundException(Guid id) => new TranslatorNotFoundException(id);
}