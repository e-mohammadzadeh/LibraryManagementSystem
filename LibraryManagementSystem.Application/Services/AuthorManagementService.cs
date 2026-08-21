using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthorManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IAuthorizationService _authorization;


	public AuthorManagementService(IAuthorRepository authorRepository, IAuthorizationService authorization)
	{
		_authorRepository = authorRepository;
		_authorization = authorization;
	}


	public ServiceResult<AuthorDto> AddAuthor(CreateAuthorDto dto)
	{
		string? warningMessage = null;

		if (_authorRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		if (_authorRepository.ExistsByEmail(dto.Email))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);

		if (_authorRepository.ExistsByPhoneNumber(dto.PhoneNumber))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);

		var existingSameName = _authorRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authorRepository.Add(newAuthor);
		return warningMessage is not null
			? ServiceResult<AuthorDto>.Warning(newAuthor.ToDto(), warningMessage)
			: ServiceResult<AuthorDto>.Ok(newAuthor.ToDto(), Messages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<AuthorDto> GetAllAuthors()
	{
		return [.. _authorRepository.GetAll().Select(author => author.ToDto())];
	}


	public ServiceResult<AuthorDto> UpdateAuthor(int authorId, UpdateAuthorDto dto)
	{
		string? warningMessage = null;

		var author = _authorRepository.FindById(authorId);
		if (author is null) return ServiceResult<AuthorDto>.Fail(Messages.AuthorUpdateFailed);

		if (IsNoOpUpdateAuthor(author, dto)) return ServiceResult<AuthorDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			var existingSameName = _authorRepository.FindByName(resolvedFirstName, resolvedLastName);
			if (existingSameName is not null && existingSameName.Id != authorId)
				warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode is not null && _authorRepository.ExistsByNationalCode(dto.NationalCode, authorId))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByNationalCode);

		if (dto.Email is not null && _authorRepository.ExistsByEmail(dto.Email, authorId))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByEmail);

		if (dto.PhoneNumber is not null && _authorRepository.ExistsByPhoneNumber(dto.PhoneNumber, authorId))
			return ServiceResult<AuthorDto>.Fail(Messages.DuplicateAuthorsNotAllowedByPhoneNumber);

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		_authorRepository.Update(author);
		return warningMessage is not null
			? ServiceResult<AuthorDto>.Warning(author.ToDto(), warningMessage)
			: ServiceResult<AuthorDto>.Ok(author.ToDto(), Messages.AuthorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateAuthor(Author author, UpdateAuthorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == author.FirstName) &&
		       (dto.LastName == null || dto.LastName == author.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == author.NationalCode) &&
		       (dto.Email == null || dto.Email == author.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == author.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == author.BirthDate) &&
		       (dto.Biography == null || dto.Biography == author.Biography);
	}


	public ServiceResult<AuthorDto> RemoveAuthor(int authorId)
	{
		var author = _authorRepository.FindById(authorId);
		if (author is null) return ServiceResult<AuthorDto>.Fail(Messages.AuthorRemoveFailed);

		if (author.BookAuthors.Count != 0) return ServiceResult<AuthorDto>.Fail(Messages.AuthorHasAssociatedBooks);

		_authorRepository.Remove(author);
		return ServiceResult<AuthorDto>.Ok(author.ToDto(), Messages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<AuthorDto> SearchAuthor(string searchItem, AuthorSearchField field)
	{
		var requiredPermission = field switch
		{
			AuthorSearchField.Name => Permission.SearchAuthorByName,
			AuthorSearchField.NationalCode => Permission.SearchAuthorByNationalCode,
			AuthorSearchField.Email => Permission.SearchAuthorByEmail,
			AuthorSearchField.PhoneNumber => Permission.SearchAuthorByPhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		if (!_authorization.HasPermission(requiredPermission) &&
		    !_authorization.HasPermission(Permission.SearchAuthor))
			return [];

		Func<Author, string?> selector = field switch
		{
			AuthorSearchField.Name => a => $"{a.FirstName} {a.LastName}",
			AuthorSearchField.NationalCode => a => a.NationalCode,
			AuthorSearchField.Email => a => a.Email,
			AuthorSearchField.PhoneNumber => a => a.PhoneNumber,
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _authorRepository.Search(searchItem, selector).Select(author => author.ToDto())];
	}


	public IReadOnlyList<BookDto> GetBooksByAuthor(int authorId)
	{
		var author = _authorRepository.FindById(authorId);
		if (author is null) return [];
		return [.. author.BookAuthors.Select(ba => ba.Book.ToDto())];
	}
}