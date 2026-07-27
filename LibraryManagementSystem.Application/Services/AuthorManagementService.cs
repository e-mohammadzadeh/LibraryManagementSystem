using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuthorManagementService
{
	private readonly IAuthorRepository _authorRepository;


	public AuthorManagementService(IAuthorRepository authorRepository)
	{
		_authorRepository = authorRepository;
	}


	public ServiceResult<AuthorDto> AddAuthor(CreateAuthorDto dto)
	{
		string? warningMessage = null;

		if (_authorRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (_authorRepository.ExistsByEmail(dto.Email))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var existingSameName = _authorRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = $"An author with the same name already exists (ID: {existingSameName.Id}).";

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authorRepository.Add(newAuthor);
		return warningMessage is not null
			? ServiceResult<AuthorDto>.Warning(MapToDto(newAuthor), warningMessage)
			: ServiceResult<AuthorDto>.Ok(MapToDto(newAuthor), ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<AuthorDto> GetAllAuthors()
	{
		return _authorRepository.GetAll().Select(MapToDto).ToList().AsReadOnly();
	}


	public AuthorDto? FindAuthorById(int id)
	{
		var author = _authorRepository.FindById(id);
		return author is null ? null : MapToDto(author);
	}


	public ServiceResult<AuthorDto> UpdateAuthor(int authorId, UpdateAuthorDto dto)
	{
		var author = _authorRepository.FindById(authorId);
		if (author is null)
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.AuthorUpdateFailed);

		if (IsNoOpUpdateAuthor(author, dto))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_authorRepository.ExistsByName(resolvedFirstName, resolvedLastName, authorId))
				return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
		}

		if (dto.NationalCode is not null && _authorRepository.ExistsByNationalCode(dto.NationalCode, authorId))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (dto.Email is not null && _authorRepository.ExistsByEmail(dto.Email, authorId))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		if (dto.PhoneNumber is not null && _authorRepository.ExistsByPhoneNumber(dto.PhoneNumber, authorId))
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		return ServiceResult<AuthorDto>.Ok(MapToDto(author), ValidationMessages.AuthorUpdatedSuccessfully);
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
		if (author is null)
			return ServiceResult<AuthorDto>.Fail(ValidationMessages.AuthorRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting author should check that none of books isn't borrowed
		if (author.BookAuthors.Count != 0)
			return ServiceResult<AuthorDto>.Fail("Failed to remove author. The author has associated books.");

		_authorRepository.Remove(author);
		return ServiceResult<AuthorDto>.Ok(MapToDto(author), ValidationMessages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<AuthorDto> SearchAuthor(string searchItem, Func<Author, string?> selector)
	{
		return _authorRepository.Search(searchItem, selector).Select(MapToDto).ToList().AsReadOnly();
	}


	internal static AuthorDto MapToDto(Author author)
	{
		return new AuthorDto
		{
			Id = author.Id,
			FirstName = author.FirstName,
			LastName = author.LastName,
			NationalCode = author.NationalCode,
			Email = author.Email,
			PhoneNumber = author.PhoneNumber,
			BirthDate = author.BirthDate,
			Biography = author.Biography,
			BookCount = author.BookAuthors.Count,
			CreatedAt = author.CreatedAt,
			UpdatedAt = author.UpdatedAt
		};
	}
}