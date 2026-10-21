using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService : IUserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _finesRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository, IPasswordHasher passwordHasher,
		IAuthorizationService authorization, IAuditLogManagementService auditLog)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_loanRepository = loanRepository;
		_finesRepository = fineRepository;
		_passwordHasher = passwordHasher;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<UserDto> AddUser(UserDto dto)
	{
		if (!_authorization.HasPermission(Permission.AddUser))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		if (_userRepository.Exists(dto.NationalCode, u => u.NationalCode))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.Exists(dto.Email, u => u.Email))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		if (_userRepository.Exists(dto.PhoneNumber, u => u.PhoneNumber))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		var role = _roleRepository.GetById(dto.Role.Id);
		if (role is null) return ServiceResult<UserDto>.Fail(Messages.NotAvailableRoles);

		if (!(role.Name switch
		    {
			    LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
			    LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
			    LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
			    _ => false
		    }))
		{
			return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);
		}

		var result = _passwordHasher.CreatePasswordHash(dto.Password);

		var newUser = new User
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate,
			RoleId = dto.Role.Id,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today)
		};

		SetPasswordHash(newUser.Id, result.Hash, result.Salt);
		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user created.");

		return ServiceResult<UserDto>.Ok(newUser.ToDto(), Messages.UserAddedSuccessfully);
	}


	public ServiceResult<UserDto> UpdateUser(UserDto dto, ICurrentUserSession session, Guid? updatedBy = null)
	{
		var user = _userRepository.GetById(dto.Id, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(Messages.NoChangesDetected);

		if (_userRepository.Exists(dto.NationalCode, u => u.NationalCode, dto.Id))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (_userRepository.Exists(dto.Email, u => u.Email, dto.Id))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		if (_userRepository.Exists(dto.PhoneNumber, u => u.PhoneNumber, dto.Id))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		var resolvedRole = _roleRepository.GetById(dto.Role.Id);
		if (resolvedRole is null) return ServiceResult<UserDto>.Fail(Messages.NotAvailableRoles);

		var allowed = resolvedRole.Name switch
		{
			LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
			LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
			LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
			_ => false
		};

		if (!allowed) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);

		user.FirstName = dto.FirstName;
		user.LastName = dto.LastName;
		user.NationalCode = dto.NationalCode;
		user.Email = dto.Email;
		user.PhoneNumber = dto.PhoneNumber;
		user.BirthDate = dto.BirthDate;
		user.RoleId = dto.Role.Id;
		user.UpdatedByUserId = updatedBy;

		var auditDetails = UserUpdateAuditDetailsBuilder.BuildUserUpdateAuditDetails(user, dto, resolvedRole);
		_userRepository.Update(user);
		if (session.CurrentUser.Id == dto.Id) session.UpdateCurrentUser(user.ToAuthUserDto());
		_auditLog.Record(AuditAction.UserUpdated, "User", dto.Id, auditDetails ?? "User updated.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateUser(User user, UserDto dto)
	{
		return (dto.FirstName == user.FirstName) &&
		       (dto.LastName == user.LastName) &&
		       (dto.NationalCode == user.NationalCode) &&
		       (dto.Email == user.Email) &&
		       (dto.PhoneNumber == user.PhoneNumber) &&
		       (dto.BirthDate == user.BirthDate) &&
		       (dto.Role.Id == user.RoleId);
	}


	public ServiceResult<UserDto> RemoveUser(Guid userId, ICurrentUserSession? session = null)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserRemoveFailed);

		if (session != null && session.CurrentUser.Id == userId)
			return ServiceResult<UserDto>.Fail(Messages.CannotRemoveYourself);

		if (session != null && !CanRemoveUser(session, user)) return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		if (_loanRepository.GetByUserId(userId, LoanFilter.Active).Count > 0)
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByActiveLoans);

		if (_finesRepository.HasFines(userId, FineFilter.Unpaid))
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByUnpaidFines);

		_userRepository.Remove(user);
		_auditLog.Record(AuditAction.UserRemoved, "User", userId, "User removed.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserRemovedSuccessfully);
	}


	private bool CanRemoveUser(ICurrentUserSession session, User user)
	{
		if (!_authorization.HasPermission(Permission.RemoveUser)) return false;

		var targetRole = _roleRepository.GetById(user.RoleId)?.Name;

		if (session.IsAdmin) return targetRole != LibraryUserRole.Admin;
		if (session.IsLibrarian) return targetRole == LibraryUserRole.Member;
		return false;
	}


	public IReadOnlyList<UserDto> SearchUser(string searchItem, UserSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		Func<User, string?> selector = field switch
		{
			UserSearchField.FullName => u => $"{u.FirstName} {u.LastName}",
			UserSearchField.NationalCode => u => u.NationalCode,
			UserSearchField.Email => u => u.Email,
			UserSearchField.PhoneNumber => u => u.PhoneNumber,
			UserSearchField.Role => u => string.Join(", ", u.RoleId), // has problem it should sort by role not role id
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return
		[
			.. _userRepository.GetAll(EntityFilter.Active)
				.Where(user =>
				{
					var value = selector(user);
					return value is not null &&
					       value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
				})
				.Select(user => user.ToDto())
		];
	}


	public IReadOnlyList<UserDto> SearchByRole(Guid roleId)
	{
		return
		[
			.. _userRepository.GetAll(EntityFilter.Active).Where(user => user.RoleId == roleId)
				.Select(user => user.ToDto())
		];
	}


	public IReadOnlyList<UserDto> GetAllUsers(UserSortField sortField = UserSortField.FullName,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		if (!_authorization.HasPermission(Permission.ViewAllUsers)) return [];

		var users = _userRepository.GetAll(EntityFilter.Active).Select(a => a.ToDto());

		Func<UserDto, object> keySelector = sortField switch
		{
			UserSortField.FirstName => u => u.FirstName,
			UserSortField.LastName => u => u.LastName,
			UserSortField.FullName => u => u.FullName,
			UserSortField.NationalCode => u => u.NationalCode,
			UserSortField.Email => u => u.Email,
			UserSortField.BirthDate => u => u.BirthDate,
			UserSortField.Role => u => u.Role.Name,
			UserSortField.MembershipStartDate => u => u.MembershipStartDate,
			UserSortField.MembershipExpiryDate => u => u.MembershipExpiryDate,
			_ => throw new ArgumentOutOfRangeException(nameof(sortField))
		};

		var sorted = sortDirection == SortDirection.Ascending
			? users.OrderBy(keySelector)
			: users.OrderByDescending(keySelector);

		return [.. sorted];
	}


	public UserDto? GetUserById(Guid id) { return _userRepository.GetById(id, EntityFilter.Active)?.ToDto(); }


	public IReadOnlyList<UserDto> GetRemovedUsers()
	{
		if (!_authorization.HasPermission(Permission.ViewRemovedUsers)) return [];
		return [.. _userRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}


	public ServiceResult<string> ChangePassword(Guid userId, string currentPassword, string newPassword,
		ICurrentUserSession session)
	{
		var isOwn = session.CurrentUser.Id == userId;

		switch (isOwn)
		{
			case true when !_authorization.HasPermission(Permission.ChangeOwnPassword):
				return ServiceResult<string>.Fail(Messages.AccessDenied);
			case false when !_authorization.HasPermission(Permission.ChangePassword):
				return ServiceResult<string>.Fail(Messages.OnlyAdminCanResetPassword);
		}

		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null || user.IsRemoved) return ServiceResult<string>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove) return ServiceResult<string>.Fail(Messages.UserFlaggedForRemoval);

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
		SetPasswordHash(userId, hashResult.Hash, hashResult.Salt);

		var message = isOwn ? Messages.PasswordChangedSuccessfully : Messages.PasswordResetSuccessfully;
		_auditLog.Record(AuditAction.UserPasswordChanged, "User", userId, "User password changed.");
		return ServiceResult<string>.Ok(user.Email, message);
	}


	public ServiceResult<UserDto> RenewMembership(Guid userId, int years)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove)
			return ServiceResult<UserDto>.Fail(string.Format(Messages.UserMarkedForRemoval,
				$"{user.FirstName} {user.LastName}"));

		if (years <= 0) return ServiceResult<UserDto>.Fail(Messages.InvalidMembershipRenewalPeriod);

		var targetRole = _roleRepository.GetById(userId)?.Name;

		if (!_authorization.HasPermission(Permission.RenewInactiveMembership) && !user.IsActive)
			return ServiceResult<UserDto>.Fail(Messages.RenewInactiveMembership);


		switch (targetRole)
		{
			case LibraryUserRole.Librarian:
			{
				if (!_authorization.HasPermission(Permission.RenewLibrarianMembership))
					return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
				break;
			}
			case LibraryUserRole.Member:
			{
				if (!_authorization.HasPermission(Permission.RenewMemberMembership))
					return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
				break;
			}
			default:
				return ServiceResult<UserDto>.Fail(Messages.AccessDenied);
		}

		RenewMembership(user, years);
		_auditLog.Record(AuditAction.MembershipRenewed, "User", userId, "User membership renewed.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.MembershipRenewedSuccessfully);
	}


	public static void RenewMembership(User user, int years = 1)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		var renewalBase = user.MembershipExpiryDate > today
			? user.MembershipExpiryDate // extend from current expiry if not yet expired
			: today; // restart from today if already expired

		user.MembershipExpiryDate = renewalBase.AddYears(years);
		if (!user.IsActive) user.IsActive = true;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void FlagForRemoval(Guid userId)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user == null) return;
		user.ShouldRemove = true;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void SetPasswordHash(Guid userId, byte[] passwordHash, byte[] passwordSalt)
	{
		if (passwordHash is null || passwordHash.Length == 0) throw new ArgumentNullException(nameof(passwordHash));
		if (passwordSalt is null || passwordSalt.Length == 0) throw new ArgumentNullException(nameof(passwordSalt));

		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user == null) return;
		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;
	}


	public void UpdateLastLogin(Guid userId)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user == null) return;
		user.LastLoginDate = user.PreviousLoginDate;
		user.PreviousLoginDate = DateTime.Now;
	}


	public void UpdateLastLoginInLogout(Guid userId)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		user?.LastLoginDate = user.PreviousLoginDate;
	}
}