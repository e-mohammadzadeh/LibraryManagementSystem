using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IAuthorRepository _authorRepository;
	private readonly IUserRepository _userRepository;


	public UserManagementService(IAuthorRepository authorRepository, IUserRepository userRepository)
	{
		_authorRepository = authorRepository;
		_userRepository = userRepository;
	}


	public ServiceResult<Author> AddAuthor(CreateAuthorDto dto)
	{
		string? warningMessage = null;

		if (_authorRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (_authorRepository.ExistsByEmail(dto.Email))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var existingSameName = _authorRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = $"An author with the same name already exists (ID: {existingSameName.Id}).";

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authorRepository.Add(newAuthor);
		return warningMessage is not null
			? ServiceResult<Author>.Warning(newAuthor, warningMessage)
			: ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authorRepository.GetAll();
	}


	public Author? FindAuthorById(int id)
	{
		return _authorRepository.FindById(id);
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
			if (_authorRepository.ExistsByName(resolvedFirstName, resolvedLastName, authorId))
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
		}

		if (dto.NationalCode is not null && _authorRepository.ExistsByNationalCode(dto.NationalCode, authorId))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (dto.Email is not null && _authorRepository.ExistsByEmail(dto.Email, authorId))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		if (dto.PhoneNumber is not null && _authorRepository.ExistsByPhoneNumber(dto.PhoneNumber, authorId))
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

		_authorRepository.Remove(author);
		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<Author> SearchAuthor(string searchItem, Func<Author, string?> selector)
	{
		return _authorRepository.Search(searchItem, selector);
	}


	public ServiceResult<User> AddUser(CreateUserDto dto)
	{
		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByNationalCode);

		if (_userRepository.ExistsByEmail(dto.Email))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByEmail);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = $"A member with the same name already exists (ID: {existingSameName.Id}). ";

		var roles = _userRoles.Where(r => dto.RoleIds.Contains(r.Id)).ToList();
		if (roles.Count != dto.RoleIds.Count)
			return ServiceResult<User>.Fail("One or more roles do not exist.");


		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email,
			dto.PhoneNumber, dto.BirthDate, roles);

		_userRepository.Add(newUser);
		return warningMessage is not null
			? ServiceResult<User>.Warning(newUser, warningMessage)
			: ServiceResult<User>.Ok(newUser, ValidationMessages.MemberAddedSuccessfully);
	}


	public IReadOnlyList<User> GetAllUsers()
	{
		return _userRepository.GetAll();
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
			if (_userRepository.ExistsByName(resolvedFirstName, resolvedLastName, userId))
				return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByName);
		}

		if (dto.NationalCode is not null && _userRepository.ExistsByNationalCode(dto.NationalCode, userId))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByNationalCode);

		if (dto.Email is not null && _userRepository.ExistsByEmail(dto.Email, userId))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByEmail);

		if (dto.PhoneNumber is not null && _userRepository.ExistsByPhoneNumber(dto.PhoneNumber, userId))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByPhoneNumber);

		if (dto.Role is not null && _userRepository.ExistsByRole(dto.Role, userId))
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateUserByRole);

		user.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate, dto.Role);
		return ServiceResult<User>.Ok(user, ValidationMessages.MemberUpdatedSuccessfully);
	}


	public User? FindUserById(int id)
	{
		return _userRepository.FindById(id);
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

		_userRepository.Remove(member);
		return ServiceResult<User>.Ok(member, ValidationMessages.MemberRemovedSuccessfully);
	}


	public IReadOnlyList<User> SearchUser(string searchItem, Func<User, string?> selector)
	{
		return _userRepository.Search(searchItem, selector);
	}


	// DeactivateMember  FindUserById
}