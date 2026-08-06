using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _fineRepository;
	private readonly IPasswordHasher _passwordHasher;


	public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository, IPasswordHasher passwordHasher)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_loanRepository = loanRepository;
		_fineRepository = fineRepository;
		_passwordHasher = passwordHasher;
	}


	public ServiceResult<UserDto> AddUser(CreateUserDto dto)
	{
		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.ExistsByEmail(dto.Email))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = $"A user with the same name already exists (ID: {existingSameName.Id}). ";

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
			return ServiceResult<UserDto>.Fail(Messages.FailureDuplicateRolesSelected);

		var roles = _roleRepository.FindByIds(dto.RoleIds);
		if (roles.Count != dto.RoleIds.Count)
			return ServiceResult<UserDto>.Fail("One or more selected roles do not exist.");

		var result = _passwordHasher.CreatePasswordHash(dto.Password);

		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			roles);

		newUser.SetPasswordHash(result.Hash, result.Salt);
		_userRepository.Add(newUser);
		return warningMessage is not null
			? ServiceResult<UserDto>.Warning(newUser.ToDto(), warningMessage)
			: ServiceResult<UserDto>.Ok(newUser.ToDto(), Messages.UserAddedSuccessfully);
	}


	public IReadOnlyList<UserDto> GetAllUsers(ICurrentUserSession session)
	{
		return session is { IsAdmin: false, IsLibrarian: false }
			? []
			: _userRepository.GetAll().Select(user => user.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<Role> GetAllRoles() { return _roleRepository.GetAllRoles(); }


	public ServiceResult<UserDto> UpdateUser(int userId, UpdateUserDto dto)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? user.FirstName;
		var resolvedLastName = dto.LastName ?? user.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_userRepository.ExistsByName(resolvedFirstName, resolvedLastName, userId))
				return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByName);
		}

		if (dto.NationalCode is not null && _userRepository.ExistsByNationalCode(dto.NationalCode, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (dto.Email is not null && _userRepository.ExistsByEmail(dto.Email, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		if (dto.PhoneNumber is not null && _userRepository.ExistsByPhoneNumber(dto.PhoneNumber, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
		{
			return ServiceResult<UserDto>.Fail(Messages.FailureDuplicateRolesSelected);
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

		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserUpdatedSuccessfully);
	}


	public UserDto? FindUserById(int id)
	{
		var user = _userRepository.FindById(id);
		return user?.ToDto();
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


	public ServiceResult<UserDto> RemoveUser(int userId, ICurrentUserSession? session = null)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserRemoveFailed);

		if (session is not null && session.UserId == userId)
			return ServiceResult<UserDto>.Fail(Messages.CannotRemoveYourself);

		if (session is not null && !CanRemoveUser(session, user))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		if (_loanRepository.CountActiveLoansByUser(userId) > 0)
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByActiveLoans);

		if (_fineRepository.HasUnpaidFines(userId))
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByUnpaidFines);

		_userRepository.Remove(user);
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserRemovedSuccessfully);
	}


	private static bool CanRemoveUser(ICurrentUserSession session, User targetUser)
	{
		var targetRoles = targetUser.UserRoles.Select(ur => ur.Role.Name).ToList();

		if (session.IsAdmin) return !targetRoles.Contains(LibraryUserRole.Admin);

		if (session.IsLibrarian)
			return targetRoles.Contains(LibraryUserRole.Member)
			       && !targetRoles.Contains(LibraryUserRole.Librarian)
			       && !targetRoles.Contains(LibraryUserRole.Admin);

		return false;
	}



	public IReadOnlyList<UserDto> SearchUser(string searchTerm, Func<User, string?> selector)
	{
		return _userRepository.Search(searchTerm, selector).Select(user => user.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<UserDto> SearchByRole(List<int> role)
	{
		return _userRepository.SearchByRole(role).Select(user => user.ToDto()).ToList().AsReadOnly();
	}


	// DeactivateMember  FindUserById
}