using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services;

public class UserManagementService
{
	private List<Author> _authors = new();
	private List<User> _users = new();
	//private readonly List<Manager> _managers = new();


	public ServiceResult<Author> AddAuthor(CreateAuthorDto dto)
	{
		string? warningMessage = null;

		if (_authors.Any(author => author.NationalCode.Equals(dto.NationalCode)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (_authors.Any(author => author.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var existingSameName = _authors.FirstOrDefault(a =>
			a.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase) &&
			a.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase));

		if (existingSameName is not null)
			warningMessage = $"An author with the same name already exists (ID: {existingSameName.Id}).";

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authors.Add(newAuthor);
		return warningMessage is not null
			? ServiceResult<Author>.Warning(newAuthor, warningMessage)
			: ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors.AsReadOnly().AsReadOnly();
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

		if (IsNoOpUpdateAuthor(author, dto))
			return ServiceResult<Author>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_authors.Any(aut =>
				    aut.Id != authorId && aut.FirstName.Equals(resolvedFirstName, StringComparison.OrdinalIgnoreCase) &&
				    aut.LastName.Equals(resolvedLastName, StringComparison.OrdinalIgnoreCase)))
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
		}

		if (dto.NationalCode is not null &&
		    _authors.Any(aut => aut.Id != authorId && aut.NationalCode.Equals(dto.NationalCode)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (dto.Email is not null && _authors.Any(aut =>
			    aut.Id != authorId && aut.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		if (dto.PhoneNumber is not null &&
		    _authors.Any(aut => aut.Id != authorId && aut.PhoneNumber.Equals(dto.PhoneNumber)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorUpdatedSuccessfully);
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


	public IReadOnlyList<Author> SearchAuthor(string searchItem, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<Author>();

		return _authors.Where(author =>
		{
			var value = selector(author);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}


	public ServiceResult<User> AddUser(CreateUserDto dto)
	{
		string? warningMessage = null;

		if (_users.Any(user => user.NationalCode.Equals(dto.NationalCode)))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByNationalCode);

		if (_users.Any(user => user.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByEmail);

		var existingSameName = _users.FirstOrDefault(user =>
			user.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase) &&
			user.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase));

		if (existingSameName is not null)
			warningMessage = $"A member with the same name already exists (ID: {existingSameName.Id}). ";

		var newMember = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email,
			dto.PhoneNumber, dto.BirthDate);

		_users.Add(newMember);
		return warningMessage is not null
			? ServiceResult<User>.Warning(newMember, warningMessage)
			: ServiceResult<User>.Ok(newMember, ValidationMessages.MemberAddedSuccessfully);
	}


	public IReadOnlyList<User> GetAllUsers()
	{
		return _users.AsReadOnly().AsReadOnly();
	}


	public ServiceResult<User> UpdateUser(int userId, UpdateUserDto dto)
	{
		var user = FindUserById(userId);
		if (user is null)
			return ServiceResult<User>.Fail(ValidationMessages.MemberUpdateFailed);

		if (IsNoOpUpdateUser(user, dto))
			return ServiceResult<User>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? user.FirstName;
		var resolvedLastName = dto.LastName ?? user.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_users.Any(m => m.Id != userId &&
			                      m.FirstName.Equals(resolvedFirstName, StringComparison.OrdinalIgnoreCase) &&
			                      m.LastName.Equals(resolvedLastName, StringComparison.OrdinalIgnoreCase)))
				return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByName);
		}

		if (dto.NationalCode is not null &&
		    _users.Any(m => m.Id != userId && m.NationalCode.Equals(dto.NationalCode)))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByNationalCode);

		if (dto.Email is not null && _users.Any(m =>
			    m.Id != userId && m.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByEmail);

		if (dto.PhoneNumber is not null &&
		    _users.Any(m => m.Id != userId && m.PhoneNumber.Equals(dto.PhoneNumber)))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateMemberByPhoneNumber);

		user.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate);
		return ServiceResult<User>.Ok(user, ValidationMessages.MemberUpdatedSuccessfully);
	}



	public User? FindUserById(int id)
	{
		return _users.FirstOrDefault(m => m.Id == id);
	}


	private static bool IsNoOpUpdateUser(User user, UpdateUserDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == user.FirstName) &&
		       (dto.LastName == null || dto.LastName == user.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == user.NationalCode) &&
		       (dto.Email == null || dto.Email == user.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == user.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == user.BirthDate);
	}


	public ServiceResult<User> RemoveUser(int memberId)
	{
		var member = FindUserById(memberId);
		if (member is null)
			return ServiceResult<User>.Fail(ValidationMessages.MemberRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting member should check that none of books isn't borrowed
		//if (member.Books.Count != 0)
		//	return ServiceResult<Member>.Fail("Failed to remove author. The author has associated books.");

		_users.Remove(member);
		return ServiceResult<User>.Ok(member, ValidationMessages.MemberRemovedSuccessfully);
	}


	public IReadOnlyList<User> SearchUser(string searchItem, Func<User, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<User>();

		return _users.Where(member =>
		{
			var value = selector(member);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}


	// DeactivateMember  FindUserById
}