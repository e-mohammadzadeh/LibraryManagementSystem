using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services;

public class UserManagementService
{
	private List<Author> _authors = new();
	private List<LibraryUser> _libraryUsers = new();


	public ServiceResult<Author> AddAuthor(string firstName, string lastName, string nationalCode, string email,
		string phoneNumber, DateOnly birthDate, string? biography)
	{
		if (_authors.Any(author => author.FirstName == firstName && author.LastName == lastName))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);


		if (_authors.Any(author => author.NationalCode == nationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		var newAuthor = new Author(firstName, lastName, nationalCode, email, phoneNumber, birthDate, biography);
		_authors.Add(newAuthor);

		return ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors;
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

		if (dto.FirstName is null || dto.LastName is null)
		{
			if (_authors.Any(aut => aut.AuthorId != authorId && aut.FirstName == dto.FirstName && aut.LastName == dto.LastName))
			{
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
			}
		}
		if (dto.NationalCode is null || _authors.Any(aut => aut.AuthorId != authorId && aut.NationalCode == dto.NationalCode))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);
		}
		if (dto.Email is null || _authors.Any(aut => aut.AuthorId != authorId && aut.Email == dto.Email))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);
		}
		if (dto.PhoneNumber is null || _authors.Any(aut => aut.AuthorId != authorId && aut.PhoneNumber == dto.PhoneNumber))
		{
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);
		}

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate, dto.Biography);
		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorUpdatedSuccessfully);
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
		}).ToList().AsReadOnly();
	}


	public void AddMember()
	{
	}
	// RegisterMember  EditMember  DeactivateMember  FindUserById
}