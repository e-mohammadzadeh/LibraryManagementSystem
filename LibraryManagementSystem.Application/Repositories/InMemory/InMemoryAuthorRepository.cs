using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryAuthorRepository : InMemoryContributorRepository<Author>, IAuthorRepository
{
	protected override Exception CreateNotFoundException(Guid id) => new AuthorNotFoundException(id);
}