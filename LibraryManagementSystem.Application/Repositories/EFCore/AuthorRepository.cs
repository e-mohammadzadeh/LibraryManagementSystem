using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class AuthorRepository : ContributorRepository<Author>, IAuthorRepository
{
	public AuthorRepository(LibraryDbContext context) : base(context)
	{

	}


	protected override Exception CreateNotFoundException(Guid id) => new AuthorNotFoundException(id);
}