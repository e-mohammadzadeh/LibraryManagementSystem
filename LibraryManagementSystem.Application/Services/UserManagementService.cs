using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.ValueObjects;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _fineRepository;
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
		_fineRepository = fineRepository;
		_passwordHasher = passwordHasher;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<UserDto> AddUser(CreateUserDto dto)
	{
		if (!_authorization.HasPermission(Permission.AddUser))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		string? warningMessage = null;

		if (_userRepository.ExistsByNationalCode(dto.NationalCode, null))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		var email = Email.Create(dto.Email);
		if (_userRepository.ExistsByEmail(email, null))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);

		var phoneNumber = PhoneNumber.Create(dto.PhoneNumber);
		if (_userRepository.ExistsByPhoneNumber(phoneNumber, null))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);

		var existingSameName = _userRepository.FindByName(dto.FirstName, dto.LastName, EntityFilter.Active);

		if (existingSameName != null)
			warningMessage = string.Format(Messages.DuplicateUserNameWarning, existingSameName.Id);

		var role = _roleRepository.FindById(dto.RoleId);
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

		var result = _passwordHasher.CreatePasswordHash(dto.Password!);

		var newUser = new User
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = email,
			PhoneNumber = phoneNumber,
			BirthDate = dto.BirthDate,
			RoleId = dto.RoleId,
			Role = role,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today)
		};

		_userRepository.SetPasswordHash(newUser, result.Hash, result.Salt);
		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user created.");

		return warningMessage != null
			? ServiceResult<UserDto>.Warning(newUser.ToDto(), warningMessage)
			: ServiceResult<UserDto>.Ok(newUser.ToDto(), Messages.UserAddedSuccessfully);
	}


	public IReadOnlyList<UserDto> GetAllUsers(UserSortField sortField = UserSortField.Id,
		SortDirection sortDirection = SortDirection.Ascending)
	{
		if (!_authorization.HasPermission(Permission.ViewAllUsers)) return [];

		var users = _userRepository.GetAll(EntityFilter.Active).Select(a => a.ToDto());
		Func<UserDto, object> keySelector = sortField switch
		{
			UserSortField.Id => u => u.Id,
			UserSortField.FirstName => u => u.FirstName,
			UserSortField.LastName => u => u.LastName,
			UserSortField.FullName => u => u.FullName,
			UserSortField.NationalCode => u => u.NationalCode,
			UserSortField.Email => u => u.Email,
			UserSortField.BirthDate => u => u.BirthDate,
			UserSortField.Role => u => u.Role,
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
		return [.. _userRepository.GetAll(EntityFilter.Removed).Select(a => a.ToDto())];
	}


	public IReadOnlyList<Role> GetAllRoles() { return _roleRepository.GetAllRoles(); }


	public ServiceResult<UserDto> UpdateUser(UpdateUserDto dto, ICurrentUserSession session)
	{
		string? warningMessage = null;

		var user = _userRepository.FindById(dto.Id, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(Messages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? user.FirstName;
		var resolvedLastName = dto.LastName ?? user.LastName;
		if (dto.FirstName != null || dto.LastName != null)
		{
			var existingSameName = _userRepository.FindByName(resolvedFirstName, resolvedLastName, EntityFilter.Active);
			if (existingSameName != null && existingSameName.Id != dto.Id)
				warningMessage = string.Format(Messages.DuplicateAuthorNameWarning, existingSameName.Id);
		}

		if (dto.NationalCode != null && _userRepository.ExistsByNationalCode(dto.NationalCode, dto.Id))
			return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByNationalCode);

		if (dto.Email != null)
		{
			var email = Email.Create(dto.Email);
			if (_userRepository.ExistsByEmail(email, dto.Id))
				return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByEmail);
		}

		if (dto.PhoneNumber != null)
		{
			var phoneNumber = PhoneNumber.Create(dto.PhoneNumber);
			if (_userRepository.ExistsByPhoneNumber(phoneNumber, dto.Id))
				return ServiceResult<UserDto>.Fail(Messages.DuplicateUsersNotAllowedByPhoneNumber);
		}

		Role? resolvedRole = null;
		if (dto.RoleId != null)
		{
			resolvedRole = _roleRepository.FindById(dto.RoleId.Value);
			if (resolvedRole is null) return ServiceResult<UserDto>.Fail(Messages.NotAvailableRoles);

			var allowed = resolvedRole.Name switch
			{
				LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
				LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
				LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
				_ => false
			};

			if (!allowed) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);
		}

		var auditDetails = UserUpdateAuditDetailsBuilder.BuildUserUpdateAuditDetails(user, dto, resolvedRole!);

		_userRepository.Update(user, session.UserId);
		_userRepository.ReplaceRole(user, resolvedRole!);
		if (session.UserId == dto.Id) session.UpdateCurrentUser(user.ToAuthUserDto());

		_auditLog.Record(AuditAction.UserUpdated, "User", dto.Id, auditDetails ?? "User updated.");

		return warningMessage != null
			? ServiceResult<UserDto>.Warning(user.ToDto(), warningMessage)
			: ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserUpdatedSuccessfully);
	}


	public UserDto? FindUserById(Guid id)
	{
		var user = _userRepository.FindById(id, EntityFilter.Active);
		return user?.ToDto();
	}


	private static bool IsNoOpUpdateUser(User user, UpdateUserDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == user.FirstName) &&
		       (dto.LastName == null || dto.LastName == user.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == user.NationalCode) &&
		       (dto.Email == null || dto.Email == user.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == user.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == user.BirthDate) &&
		       (dto.RoleId == null || dto.RoleId == user.RoleId);
	}


	public ServiceResult<UserDto> RemoveUser(Guid userId, ICurrentUserSession? session = null)
	{
		var user = _userRepository.FindById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserRemoveFailed);

		if (session != null && session.UserId == userId)
			return ServiceResult<UserDto>.Fail(Messages.CannotRemoveYourself);

		if (session != null && !CanRemoveUser(session, user))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		if (_loanRepository.CountLoans(userId, LoanFilter.Active) > 0)
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByActiveLoans);

		if (_fineRepository.HasFines(userId, FineFilter.Unpaid))
			return ServiceResult<UserDto>.Fail(Messages.UserRemovalFailedByUnpaidFines);

		_userRepository.Remove(user);
		_auditLog.Record(AuditAction.UserRemoved, "User", userId, "User removed.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserRemovedSuccessfully);
	}


	private bool CanRemoveUser(ICurrentUserSession session, User user)
	{
		if (!_authorization.HasPermission(Permission.RemoveUser)) return false;

		var targetRole = user.Role.Name;

		if (session.IsAdmin) return targetRole != LibraryUserRole.Admin;
		if (session.IsLibrarian) return targetRole == LibraryUserRole.Member;
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
			UserSearchField.Role => u => string.Join(", ", u.Role.Name),
			_ => throw new ArgumentOutOfRangeException(nameof(field))
		};

		return [.. _userRepository.Search(searchTerm, selector).Select(user => user.ToDto())];
	}


	public IReadOnlyList<UserDto> SearchByRole(Guid roleId)
	{
		return [.. _userRepository.SearchByRole(roleId).Select(user => user.ToDto())];
	}


	public ServiceResult<string> ChangePassword(Guid userId, string currentPassword, string newPassword,
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

		var user = _userRepository.FindById(userId, EntityFilter.Active);
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
		_userRepository.SetPasswordHash(user, hashResult.Hash, hashResult.Salt);

		var message = isOwn ? Messages.PasswordChangedSuccessfully : Messages.PasswordResetSuccessfully;
		_auditLog.Record(AuditAction.UserPasswordChanged, "User", userId, "User password changed.");
		return ServiceResult<string>.Ok(user.Email, message);
	}


	public ServiceResult<UserDto> RenewMembership(Guid userId, int years)
	{
		var user = _userRepository.FindById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove)
			return ServiceResult<UserDto>.Fail(string.Format(Messages.UserMarkedForRemoval,
				$"{user.FirstName} {user.LastName}"));

		if (years <= 0) return ServiceResult<UserDto>.Fail(Messages.InvalidMembershipRenewalPeriod);

		var targetRole = user.Role.Name;

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

		_userRepository.RenewMembership(user, years);
		_auditLog.Record(AuditAction.MembershipRenewed, "User", userId, "User membership renewed.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.MembershipRenewedSuccessfully);
	}




	public void ReplaceRole(User user, Role newRole) {
		user.Role = newRole ?? throw new ArgumentNullException(nameof(newRole));
		user.RoleId = newRole.Id;
	}


	public void RenewMembership(User user, int years = 1) {
		var today = DateOnly.FromDateTime(DateTime.Today);
		var renewalBase = user.MembershipExpiryDate > today
			? user.MembershipExpiryDate // extend from current expiry if not yet expired
			: today; // restart from today if already expired

		user.MembershipExpiryDate = renewalBase.AddYears(years);
		if (!user.IsActive)
			user.IsActive = true;
		user.UpdatedAt = DateTime.UtcNow;
	}



	public void FlagForRemoval(User user) {
		user.ShouldRemove = true;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void SetPasswordHash(User user, byte[] passwordHash, byte[] passwordSalt) {
		if (passwordHash is null || passwordHash.Length == 0)
			throw new ArgumentNullException(nameof(passwordHash));
		if (passwordSalt is null || passwordSalt.Length == 0)
			throw new ArgumentNullException(nameof(passwordSalt));

		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;
	}


	public void UpdateLastLogin(User user) {
		user.LastLoginDate = user.PreviousLoginDate;
		user.PreviousLoginDate = DateTime.Now;
	}


	public void UpdateLastLoginInLogout(User user) {
		user.LastLoginDate = user.PreviousLoginDate;
	}

}