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

		if (IsDuplicateEmail(dto.Email))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate, dto.Biography);
		_authors.Add(newAuthor);

		return ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors.AsReadOnly();
	}


	private Author? FindAuthorById(int id)
	{
		return _authors.FirstOrDefault(a => a.AuthorId == id);
	}


	public ServiceResult<Author> UpdateAuthor(int authorId, UpdateAuthorDto dto)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorUpdateFailed);

		if (dto.FirstName != null || dto.LastName != null)
		{
			if (_authors.Any(aut => aut.AuthorId != authorId) && IsDuplicateFirstName(dto.FirstName) &&
			    IsDuplicateLastName(dto.LastName))
			{
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
			}
		}

		if (dto.NationalCode != null &&
		    _authors.Any(aut => aut.AuthorId != authorId && aut.NationalCode == dto.NationalCode))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);
		}

		if (dto.Email != null && (_authors.Any(aut => aut.AuthorId != authorId) && IsDuplicateEmail(dto.Email)))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);
		}

		if (dto.PhoneNumber != null &&
		    _authors.Any(aut => aut.AuthorId != authorId && aut.PhoneNumber == dto.PhoneNumber))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);
		}

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorUpdatedSuccessfully);
	}


	public ServiceResult<Author> RemoveAuthor(int authorId)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting author should check that none of books isn't borrowed
		if (author.Books.Any())
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


	private bool IsDuplicateFirstName(string? name)
	{
		return _authors.Any(author => author.FirstName.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	private bool IsDuplicateLastName(string? name)
	{
		return _authors.Any(author => author.LastName.Equals(name, StringComparison.OrdinalIgnoreCase));
	}


	private bool IsDuplicateEmail(string email)
	{
		return _authors.Any(author => author.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}


	public void AddMember()
	{
	}
	// RegisterMember  EditMember  DeactivateMember  FindUserById
}