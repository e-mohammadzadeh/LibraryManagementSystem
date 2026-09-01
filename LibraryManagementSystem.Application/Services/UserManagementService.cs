using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Enums.Search;
using LibraryManagementSystem.Domain.Enums.Sort;
using LibraryManagementSystem.Domain.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _fineRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IAuthorizationService _authorization;


	public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository, IPasswordHasher passwordHasher,
		IAuthorizationService authorization)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_loanRepository = loanRepository;
		_fineRepository = fineRepository;
		_passwordHasher = passwordHasher;
		_authorization = authorization;
	}


	public ServiceResult<UserDto> AddUser(CreateUserDto dto)
	{
		if (!_authorization.HasPermission(Permission.AddUser))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.ExistsByEmail(dto.Email))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName);

		if (existingSameName is not null)
			warningMessage = string.Format(Messages.DuplicateUserNameWarning, existingSameName.Id);

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
			return ServiceResult<UserDto>.Fail(Messages.FailureDuplicateRolesSelected);

		var roles = _roleRepository.FindByIds(dto.RoleIds);
		if (roles.Count != dto.RoleIds.Count) return ServiceResult<UserDto>.Fail(Messages.NotAvailableRoles);

		if (roles.Select(role => role.Name switch
		    {
			    LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
			    LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
			    LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
			    _ => false
		    }).Any(allowed => !allowed))
		{
			return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);
		}

		var result = _passwordHasher.CreatePasswordHash(dto.Password!);

		var newUser = new User(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			roles);

		newUser.SetPasswordHash(result.Hash, result.Salt);
		_userRepository.Add(newUser);
		return warningMessage is not null
			? ServiceResult<UserDto>.Warning(newUser.ToDto(), warningMessage)
			: ServiceResult<UserDto>.Ok(newUser.ToDto(), Messages.UserAddedSuccessfully);
	}


	public IReadOnlyList<UserDto> GetAllUsers(UserSortField sortField = UserSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		if (!_authorization.HasPermission(Permission.ViewAllUsers)) return [];

		var users = _userRepository.GetAll(UserFilter.Active).Select(a => a.ToDto());
		Func<UserDto, object> keySelector = sortField switch
		{
			UserSortField.Id => u => u.Id,
			UserSortField.FirstName => u => u.FirstName,
			UserSortField.LastName => u => u.LastName,
			UserSortField.FullName => u => u.FullName,
			UserSortField.NationalCode => u => u.NationalCode,
			UserSortField.Email => u => u.Email,
			UserSortField.BirthDate => u => u.BirthDate,
			UserSortField.Roles => u => u.Roles,
			UserSortField.MembershipStartDate => u => u.MembershipStartDate,
			UserSortField.MembershipExpiryDate => u => u.MembershipExpiryDate,
			UserSortField.IsActive => u => u.IsActive,
			UserSortField.LastLoginDate => u => u.LastLoginDate ?? (object)DateTime.MinValue,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? users.OrderBy(keySelector)
			: users.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public IReadOnlyList<UserDto> GetRemovedUsers()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedUsers)) return [];
		return [.. _userRepository.GetAll(UserFilter.Removed).Select(a => a.ToDto())];
	}


	public IReadOnlyList<Role> GetAllRoles() { return _roleRepository.GetAllRoles(); }


	public ServiceResult<UserDto> UpdateUser(int userId, UpdateUserDto dto)
	{
		string? warningMessage = null;

		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? user.FirstName;
		var resolvedLastName = dto.LastName ?? user.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			var existingSameName = _userRepository.FindByName(resolvedFirstName, resolvedLastName);
			if (existingSameName is not null && existingSameName.Id != userId)
				warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode is not null && _userRepository.ExistsByNationalCode(dto.NationalCode, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (dto.Email is not null && _userRepository.ExistsByEmail(dto.Email, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		if (dto.PhoneNumber is not null && _userRepository.ExistsByPhoneNumber(dto.PhoneNumber, userId))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
			return ServiceResult<UserDto>.Fail(Messages.FailureDuplicateRolesSelected);

		List<Role>? resolvedRoles = null;
		if (dto.RoleIds is { Count: > 0 })
		{
			if (dto.RoleIds.Count != dto.RoleIds.Distinct().Count())
				return ServiceResult<UserDto>.Fail(Messages.FailureDuplicateRolesSelected);

			resolvedRoles = [.. _roleRepository.FindByIds(dto.RoleIds)];
			if (resolvedRoles.Count != dto.RoleIds.Count)
				return ServiceResult<UserDto>.Fail(Messages.NotAvailableRoles);

			if (resolvedRoles.Select(role => role.Name switch
			    {
				    LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
				    LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
				    LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
				    _ => false
			    }).Any(allowed => !allowed))
			{
				return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);
			}
		}

		user.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			resolvedRoles);
		_userRepository.Update(user);

		return warningMessage is not null
			? ServiceResult<UserDto>.Warning(user.ToDto(), warningMessage)
			: ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserUpdatedSuccessfully);
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


	private bool CanRemoveUser(ICurrentUserSession session, User targetUser)
	{
		if (!_authorization.HasPermission(Permission.RemoveUser)) return false;

		var targetRoles = targetUser.UserRoles.Select(ur => ur.Role.Name).ToList();

		if (session.IsAdmin) return !targetRoles.Contains(LibraryUserRole.Admin);

		if (session.IsLibrarian)
			return targetRoles.Contains(LibraryUserRole.Member)
			       && !targetRoles.Contains(LibraryUserRole.Librarian)
			       && !targetRoles.Contains(LibraryUserRole.Admin);

		return false;
	}


	public IReadOnlyList<UserDto> SearchUser(string searchTerm, UserSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return [];
		Func<User, string?> selector = field switch
		{
			UserSearchField.FullName => u => $"{u.FirstName} {u.LastName}",
			UserSearchField.NationalCode => u => u.NationalCode,
			UserSearchField.Email => u => u.Email,
			UserSearchField.PhoneNumber => u => u.PhoneNumber,
			UserSearchField.Role => u => string.Join(", ", u.UserRoles.Select(r => r.Role.Name)),
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _userRepository.Search(searchTerm, selector).Select(user => user.ToDto())];
	}


	public IReadOnlyList<UserDto> SearchByRole(IReadOnlyList<int> roleIds)
	{
		return [.. _userRepository.SearchByRole(roleIds).Select(user => user.ToDto())];
	}


	public ServiceResult<string> ChangePassword(int userId, string currentPassword, string newPassword,
		ICurrentUserSession session)
	{
		var isOwn = session.UserId == userId;

		switch (isOwn)
		{
			case true when !_authorization.HasPermission(Permission.ChangeOwnPassword):
				return ServiceResult<string>.Fail(Messages.AccessDenied);
			case false when !_authorization.HasPermission(Permission.ChangePassword):
				return ServiceResult<string>.Fail(Messages.OnlyAdminCanResetPassword);
		}

		var user = _userRepository.FindById(userId);
		if (user is null || user.IsRemoved) return ServiceResult<string>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove)
			return ServiceResult<string>.Fail(Messages.UserFlaggedForRemoval);

		// Own change: verify current password
		if (isOwn)
		{
			if (string.IsNullOrWhiteSpace(currentPassword) ||
			    !_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
				return ServiceResult<string>.Fail(Messages.PasswordChangeFailed);

			if (newPassword == currentPassword) return ServiceResult<string>.Fail(Messages.SelectDifferentNewPassword);
		}

		if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < ValidationConstants.MinPasswordLength)
			return ServiceResult<string>.Fail(Messages.MinimumPasswordLength);

		var hashResult = _passwordHasher.CreatePasswordHash(newPassword);
		user.SetPasswordHash(hashResult.Hash, hashResult.Salt);
		_userRepository.Update(user);

		var message = isOwn ? Messages.PasswordChangedSuccessfully : Messages.PasswordResetSuccessfully;

		return ServiceResult<string>.Ok(user.Email, message);
	}


	public ServiceResult<UserDto> RenewMembership(int userId, int years)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove)
			return ServiceResult<UserDto>.Fail(string.Format(Messages.UserMarkedForRemoval,
				$"{user.FirstName} {user.LastName}"));

		if (years <= 0) return ServiceResult<UserDto>.Fail(Messages.InvalidMembershipRenewalPeriod);

		var targetRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

		if (!_authorization.HasPermission(Permission.RenewInactiveMembership) && !user.IsActive)
			return ServiceResult<UserDto>.Fail(Messages.RenewInactiveMembership);


		if (targetRoles.Contains(LibraryUserRole.Librarian))
		{
			if (!_authorization.HasPermission(Permission.RenewLibrarianMembership))
				return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
		}
		else if (targetRoles.Contains(LibraryUserRole.Member))
		{
			if (!_authorization.HasPermission(Permission.RenewMemberMembership))
				return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
		}
		else
		{
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
		}

		user.RenewMembership(years);
		_userRepository.Update(user);
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.MembershipRenewedSuccessfully);
	}
}