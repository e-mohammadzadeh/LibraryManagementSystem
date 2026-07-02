using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services;

public class UserManagementService
{
	private List<Author> _authors = new();
	private List<LibraryUser> _libraryUsers = new();


	public ServiceResult<Author> AddAuthor(CreateAuthorDto dto)
	{
		if (_authors.Any(author => author.NationalCode == dto.NationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (_authors.Any(author => author.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var existingSameName = _authors.FirstOrDefault(a =>
			a.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase) &&
			a.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase));

		if (existingSameName != null)
			return ServiceResult<Author>.Warning(
				$"An author with the same name already exists (ID: {existingSameName.Id}). ");

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authors.Add(newAuthor);
		return ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors.AsReadOnly();
	}


	public Author? FindAuthorById(int id)
	{
		return _authors.FirstOrDefault(a => a.Id == id);
	}


	public ServiceResult<Author> UpdateAuthor(int authorId, UpdateAuthorDto dto)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorUpdateFailed);

		if (IsNoOpUpdate(author, dto))
			return ServiceResult<Author>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName != null || dto.LastName != null)
		{
			if (_authors.Any(aut =>
				    aut.Id != authorId && aut.FirstName == resolvedFirstName && aut.LastName == resolvedLastName))
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
		}

		if (dto.NationalCode != null &&
		    _authors.Any(aut => aut.Id != authorId && aut.NationalCode == dto.NationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (dto.Email != null && _authors.Any(aut =>
			    aut.Id != authorId && aut.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		if (dto.PhoneNumber != null &&
		    _authors.Any(aut => aut.Id != authorId && aut.PhoneNumber == dto.PhoneNumber))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdate(Author author, UpdateAuthorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == author.FirstName) &&
		       (dto.LastName == null || dto.LastName == author.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == author.NationalCode) &&
		       (dto.Email == null || dto.Email == author.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == author.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == author.BirthDate) &&
		       (dto.Biography == null || dto.Biography == author.Biography);
	}


	public ServiceResult<Author> RemoveAuthor(int authorId)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting author should check that none of books isn't borrowed
		if (author.Books.Count != 0)
			return ServiceResult<Author>.Fail("Failed to remove author. The author has associated books.");

		_authors.Remove(author);
		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<Author> SearchAuthors(string searchItem, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<Author>();

		return _authors.Where(author =>
		{
			var value = selector(author);
			return value != null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}


	public void AddMember()
	{
	}
	// RegisterMember  EditMember  DeactivateMember  FindUserById
}