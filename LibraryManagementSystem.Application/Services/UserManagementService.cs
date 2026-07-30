using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _fineRepository;


	public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_loanRepository = loanRepository;
		_fineRepository = fineRepository;
	}


	public ServiceResult<UserDto> AddUser(CreateUserDto dto)
	{
		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByNationalCode);

		if (_userRepository.ExistsByEmail(dto.Email))
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByEmail);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = $"A user with the same name already exists (ID: {existingSameName.Id}). ";

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateRolesSelected);

		var roles = _roleRepository.FindByIds(dto.RoleIds);
		if (roles.Count != dto.RoleIds.Count)
		{
			return ServiceResult<UserDto>.Fail("One or more selected roles do not exist.");
		}

		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			roles);

		_userRepository.Add(newUser);
		return warningMessage is not null
			? ServiceResult<UserDto>.Warning(MapToDto(newUser), warningMessage)
			: ServiceResult<UserDto>.Ok(MapToDto(newUser), ValidationMessages.UserAddedSuccessfully);
	}


	public IReadOnlyList<UserDto> GetAllUsers()
	{
		return _userRepository.GetAll().Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<Role> GetAllRoles() { return _roleRepository.GetAllRoles(); }


	public ServiceResult<UserDto> UpdateUser(int userId, UpdateUserDto dto)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(ValidationMessages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? user.FirstName;
		var resolvedLastName = dto.LastName ?? user.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_userRepository.ExistsByName(resolvedFirstName, resolvedLastName, userId))
				return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByName);
		}

		if (dto.NationalCode is not null && _userRepository.ExistsByNationalCode(dto.NationalCode, userId))
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByNationalCode);

		if (dto.Email is not null && _userRepository.ExistsByEmail(dto.Email, userId))
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByEmail);

		if (dto.PhoneNumber is not null && _userRepository.ExistsByPhoneNumber(dto.PhoneNumber, userId))
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateUserByPhoneNumber);

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
		{
			return ServiceResult<UserDto>.Fail(ValidationMessages.FailureDuplicateRolesSelected);
		}

		List<Role>? resolvedRoles = null;
		if (dto.RoleIds.Count != 0)
		{
			resolvedRoles = [.. _roleRepository.FindByIds(dto.RoleIds)];
			if (resolvedRoles.Count != dto.RoleIds.Count)
			{
				return ServiceResult<UserDto>.Fail("One or more selected roles do not exist.");
			}
		}


		user.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			resolvedRoles);

		return ServiceResult<UserDto>.Ok(MapToDto(user), ValidationMessages.UserUpdatedSuccessfully);
	}


	public UserDto? FindUserById(int id)
	{
		var user = _userRepository.FindById(id);
		return user is null ? null : MapToDto(user);
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


	public ServiceResult<UserDto> RemoveUser(int userId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(ValidationMessages.UserRemoveFailed);


		if (_loanRepository.CountActiveLoansByUser(userId) > 0)
			return ServiceResult<UserDto>.Fail("Failed to remove author. The author has associated books.");

		_userRepository.Remove(user);
		return ServiceResult<UserDto>.Ok(MapToDto(user), ValidationMessages.UserRemovedSuccessfully);
	}


	public IReadOnlyList<UserDto> SearchUser(string searchTerm, Func<User, string?> selector)
	{
		return _userRepository.Search(searchTerm, selector).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<UserDto> SearchByRole(List<int> role)
	{
		return _userRepository.SearchByRole(role).Select(MapToDto).ToList().AsReadOnly();
	}


	// DeactivateMember  FindUserById


	private static UserDto MapToDto(User user)
	{
		return new UserDto
		{
			Id = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			NationalCode = user.NationalCode,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			BirthDate = user.BirthDate,
			Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList().AsReadOnly(),
			MembershipStartDate = user.MembershipStartDate,
			MembershipExpiryDate = user.MembershipExpiryDate,
			IsActive = user.IsActive,
			CreatedAt = user.CreatedAt,
			UpdatedAt = user.UpdatedAt
		};
	}
}