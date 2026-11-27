using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Sort;

namespace LibraryManagementSystem.Application.Services;

public class AuthorManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IAuthorizationService _authorization;
	private readonly IBookAuthorRepository _bookAuthorRepository;


	public AuthorManagementService(IAuthorRepository authorRepository, 
		IAuthorizationService authorization, 
		IBookAuthorRepository bookAuthorRepository)
	{
		_authorRepository = authorRepository;
		_authorization = authorization;
		_bookAuthorRepository = bookAuthorRepository;
	}


	public IReadOnlyList<ContributorDto> GetAllAuthors(ContributorSortField sortField = ContributorSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		var authors = _authorRepository.GetAll(EntityFilter.Active).Select(author => author.ToDto());

		Func<ContributorDto, object> keySelector = sortField switch
		{
			ContributorSortField.FirstName => dto => dto.FirstName,
			ContributorSortField.LastName => dto => dto.LastName,
			ContributorSortField.FullName => dto => $"{dto.FirstName} {dto.LastName}",
			ContributorSortField.NationalCode => dto => dto.NationalCode,
			ContributorSortField.Email => dto => dto.Email,
			ContributorSortField.BirthDate => dto => dto.BirthDate,
			ContributorSortField.BookCount => dto => GetBooksByAuthor(dto.Id).Count,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? authors.OrderBy(keySelector)
			: authors.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(Guid authorId)
	{
		return
		[
			.. _bookAuthorRepository
				.GetByAuthorId(authorId, EntityFilter.Active)
				.Select(bookAuthor => bookAuthor.Book.ToDto())
		];
	}


	public IReadOnlyList<ContributorDto> GetRemovedAuthors()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedContributors)) return [];

		return
		[
			.._authorRepository
				.GetAll(EntityFilter.Removed)
				.Select(author => author.ToDto())
		];
	}
}