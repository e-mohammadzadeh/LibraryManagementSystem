using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;


	public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
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
			warningMessage = $"A user with the same name already exists (ID: {existingSameName.Id}). ";

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateRolesSelected);

		var roles = _roleRepository.FindByIds(dto.RoleIds);
		if (roles.Count != dto.RoleIds.Count)
		{
			return ServiceResult<User>.Fail("One or more selected roles do not exist.");
		}

		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			roles);

		_userRepository.Add(newUser);
		return warningMessage is not null
			? ServiceResult<User>.Warning(newUser, warningMessage)
			: ServiceResult<User>.Ok(newUser, ValidationMessages.UserAddedSuccessfully);
	}


	public IReadOnlyList<User> GetAllUsers()
	{
		return _userRepository.GetAll();
	}


	public IReadOnlyList<Role> GetAllRoles()
	{
		return _roleRepository.GetAllRoles();
	}


	public ServiceResult<User> UpdateUser(int userId, UpdateUserDto dto)
	{
		var user = FindUserById(userId);
		if (user is null)
			return ServiceResult<User>.Fail(ValidationMessages.UserUpdateFailed);

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

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
		{
			return ServiceResult<User>.Fail(ValidationMessages.FailureDuplicateRolesSelected);
		}

		List<Role>? resolvedRoles = null;
		if (dto.RoleIds.Count != 0)
		{
			resolvedRoles = [.. _roleRepository.FindByIds(dto.RoleIds)];
			if (resolvedRoles.Count != dto.RoleIds.Count)
			{
				return ServiceResult<User>.Fail("One or more selected roles do not exist.");
			}
		}


		user.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			resolvedRoles);

		return ServiceResult<User>.Ok(user, ValidationMessages.UserUpdatedSuccessfully);
	}


	public User? FindUserById(int id)
	{
		return _userRepository.FindById(id);
	}


	private static bool IsNoOpUpdateUser(User user, UpdateUserDto dto)
	{
		var roleChanged = dto.RoleIds.Count != 0 && !dto.RoleIds.OrderBy(i => i)
			.SequenceEqual(user.UserRoles.Select(ur => ur.RoleId).OrderBy(i => i));

		return (dto.FirstName == null || dto.FirstName == user.FirstName) &&
		       (dto.LastName == null || dto.LastName == user.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == user.NationalCode) &&
		       (dto.Email == null || dto.Email == user.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == user.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == user.BirthDate) && !roleChanged;
	}


	public ServiceResult<User> RemoveUser(int userId)
	{
		var user = FindUserById(userId);
		if (user is null)
			return ServiceResult<User>.Fail(ValidationMessages.UserRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting member should check that none of books isn't borrowed
		//if (member.Books.Count != 0)
		//	return ServiceResult<Member>.Fail("Failed to remove author. The author has associated books.");

		_userRepository.Remove(user);
		return ServiceResult<User>.Ok(user, ValidationMessages.UserRemovedSuccessfully);
	}


	public IReadOnlyList<User> SearchUser(string searchItem, Func<User, string?> selector)
	{
		return _userRepository.Search(searchItem, selector);
	}


	// DeactivateMember  FindUserById
}