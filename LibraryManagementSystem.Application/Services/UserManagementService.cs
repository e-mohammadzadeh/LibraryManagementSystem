using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Application.Validators;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;


namespace LibraryManagementSystem.Application.Services;

public class UserManagementService : IUserManagementService
{
	private readonly PersonManagementService _personService;
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _finesRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;
	protected readonly PersonUniquenessChecker _uniquenessChecker;


	public UserManagementService(PersonManagementService personService, IUserRepository userRepository,
		IRoleRepository roleRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository, IPasswordHasher passwordHasher,
		IAuthorizationService authorization, IAuditLogManagementService auditLog,
		PersonUniquenessChecker uniquenessChecker)
	{
		_personService = personService;
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_loanRepository = loanRepository;
		_finesRepository = fineRepository;
		_passwordHasher = passwordHasher;
		_authorization = authorization;
		_auditLog = auditLog;
		_uniquenessChecker = uniquenessChecker;
	}


	public ServiceResult<UserDto> AddUser(UserDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		if (!_authorization.HasPermission(Permission.AddUser))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		var dateValidation = Validator.UserBirthDateValidator(dto.BirthDate);
		if (!dateValidation.IsValid) return ServiceResult<UserDto>.Fail(dateValidation.ErrorMessage!);

		if (!CanAssignRole(dto.RoleName)) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);

		var role = _roleRepository.GetByName(dto.RoleName);
		if (role is null) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);

		var personDto = ToContributorDto(dto);
		var personResult = _personService.ResolveOrCreatePerson(personDto);
		if (!personResult.IsSuccess) return ServiceResult<UserDto>.Fail(personResult.Message!);
		var person = personResult.Data!;

		var existingUser = _userRepository.GetAll(EntityFilter.All).FirstOrDefault(user => user.PersonId == person.Id);
		if (existingUser is not null)
		{
			if (!existingUser.IsRemoved) return ServiceResult<UserDto>.Fail("This person already has a user account");

			return ServiceResult<UserDto>.Fail("This user account has been removed and requires admin restoration.");
		}

		var passwordResult = _passwordHasher.CreatePasswordHash(dto.Password);

		var newUser = new User
		{
			PersonId = person.Id,
			Person = person,
			RoleId = role.Id,
			Role = role,
			IsActive = true,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today),
			PasswordHash = passwordResult.Hash,
			PasswordSalt = passwordResult.Salt
		};

		_userRepository.Add(newUser);
		_auditLog.Record(AuditAction.UserCreated, "User", newUser.Id, "New user role created.");

		return ServiceResult<UserDto>.Ok(newUser.ToDto(), Messages.UserAddedSuccessfully);
	}


	public ServiceResult<UserDto> UpdateUser(UserDto dto, ICurrentUserSession session, Guid? updatedBy = null)
	{
		ArgumentNullException.ThrowIfNull(dto);
		ArgumentNullException.ThrowIfNull(session);

		var user = _userRepository.GetById(dto.Id, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserUpdateFailed);

		if (IsNoOpUpdateUser(user, dto)) return ServiceResult<UserDto>.Fail(Messages.NoChangesDetected);

		var roleChanged = user.Role.Name != dto.RoleName;
		Role? requestedRole = null;
		if (roleChanged)
		{
			if (!CanAssignRole(dto.RoleName)) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);

			requestedRole = _roleRepository.GetByName(dto.RoleName);
			if (requestedRole is null) return ServiceResult<UserDto>.Fail(Messages.CanOnlyAssignAllowedRoles);
		}

		var auditBuilder = new UserUpdateAuditDetailsBuilder(_roleRepository);
		var auditDetails = auditBuilder.BuildUserUpdateAuditDetails(user, dto);

		var personDto = ToContributorDto(dto);

		var personResult = _personService.UpdatePerson(user.PersonId, personDto, updatedBy);
		if (!personResult.IsSuccess) return ServiceResult<UserDto>.Fail(personResult.Message!);

		if (roleChanged)
		{
			user.RoleId = requestedRole!.Id;
			user.Role = requestedRole;
		}

		user.UpdatedByUserId = updatedBy;

		_userRepository.Update(user);

		if (session.CurrentUser.Id == dto.Id) session.UpdateCurrentUser(user.ToAuthUserDto());
		_auditLog.Record(AuditAction.UserUpdated, "User", dto.Id, auditDetails ?? "User updated.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateUser(User user, UserDto dto)
	{
		var person = user.Person;

		return (dto.FirstName == person.FirstName) &&
		       (dto.LastName == person.LastName) &&
		       (dto.NationalCode == person.NationalCode) &&
		       (dto.Email == person.Email) &&
		       (dto.PhoneNumber == person.PhoneNumber) &&
		       (dto.BirthDate == person.BirthDate) &&
		       (dto.RoleName == user.Role.Name);
	}


	public ServiceResult<UserDto> RemoveUser(Guid userId, ICurrentUserSession? session = null)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserRemoveFailed);

		if (session is not null && session.CurrentUser.Id == userId)
			return ServiceResult<UserDto>.Fail(Messages.CannotRemoveYourself);

		if (session is not null && !CanRemoveUser(session, user))
			return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

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

		if (session.IsAdmin) return user.Role.Name != LibraryUserRole.Admin;
		if (session.IsLibrarian) return user.Role.Name == LibraryUserRole.Member;
		return false;
	}


	public IReadOnlyList<UserDto> SearchUser(string searchItem, UserSearchField field)
	{
		if (string.IsNullOrWhiteSpace(searchItem)) return [];

		Func<User, string?> selector = field switch
		{
			UserSearchField.FullName => user => $"{user.Person.FirstName} {user.Person.LastName}",
			UserSearchField.NationalCode => user => user.Person.NationalCode,
			UserSearchField.Email => user => user.Person.Email,
			UserSearchField.PhoneNumber => user => user.Person.PhoneNumber,
			UserSearchField.Role => user => user.Role.Name.ToString(),
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
			.. _userRepository
				.GetAll(EntityFilter.Active)
				.Where(user => user.Role.Id == roleId)
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
			UserSortField.Email => u => u.Email.Value,
			UserSortField.BirthDate => u => u.BirthDate,
			UserSortField.Role => u => u.RoleName,
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


	public ServiceResult<string> ChangePassword(Guid userId, string? currentPassword, string newPassword,
		ICurrentUserSession session)
	{
		ArgumentNullException.ThrowIfNull(session);

		var isOwn = session.CurrentUser.Id == userId;

		if (!isOwn && !_authorization.HasPermission(Permission.ChangePassword))
			return ServiceResult<string>.Fail(Messages.OnlyAdminCanResetPassword);

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
		return ServiceResult<string>.Ok(user.Person.Email, message);
	}


	public ServiceResult<UserDto> RenewMembership(Guid userId, int years)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserNotFound);

		if (user.ShouldRemove)
			return ServiceResult<UserDto>.Fail(string.Format(Messages.UserMarkedForRemoval,
				$"{user.Person.FirstName} {user.Person.LastName}"));

		if (years <= 0) return ServiceResult<UserDto>.Fail(Messages.InvalidMembershipRenewalPeriod);

		if (!_authorization.HasPermission(Permission.RenewInactiveMembership) && !user.IsActive)
			return ServiceResult<UserDto>.Fail(Messages.RenewInactiveMembership);


		switch (user.Role.Name)
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
		_userRepository.Update(user);
		_auditLog.Record(AuditAction.MembershipRenewed, "User", userId, "User membership renewed.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.MembershipRenewedSuccessfully);
	}


	private static void RenewMembership(User user, int years = 1)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		var renewalBase = user.MembershipExpiryDate > today
			? user.MembershipExpiryDate // extend from current expiry if not yet expired
			: today; // restart from today if already expired

		user.MembershipExpiryDate = renewalBase.AddYears(years);
		user.IsActive = true;
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
		ArgumentNullException.ThrowIfNull(passwordHash);
		ArgumentNullException.ThrowIfNull(passwordSalt);

		if (passwordHash.Length == 0) throw new ArgumentException("Password hash cannot be empty.");

		if (passwordSalt.Length == 0) throw new ArgumentException("Password salt cannot be empty.");

		var user = _userRepository.GetById(userId, EntityFilter.Active) ?? throw new UserNotFoundException(userId);

		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;

		_userRepository.Update(user);
	}


	public void UpdateLastLogin(Guid userId)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user == null) return;
		user.LastLoginDate = user.PreviousLoginDate;
		user.PreviousLoginDate = DateTime.Now;
		_userRepository.Update(user);
	}


	public void UpdateLastLoginInLogout(Guid userId)
	{
		var user = _userRepository.GetById(userId, EntityFilter.Active);
		if (user is null) return;
		user.LastLoginDate = user.PreviousLoginDate;
		_userRepository.Update(user);
	}



	public bool CanAssignRole(LibraryUserRole role)
	{
		return role switch
		{
			LibraryUserRole.Member => _authorization.HasPermission(Permission.AssignMemberRole),
			LibraryUserRole.Librarian => _authorization.HasPermission(Permission.AssignLibrarianRole),
			LibraryUserRole.Admin => _authorization.HasPermission(Permission.AssignAdminRole),
			_ => false
		};
	}


	public ServiceResult<UserDto> RestoreUser(Guid userId, ICurrentUserSession session, Guid? updatedByUserId = null)
	{
		if (!session.IsAdmin) return ServiceResult<UserDto>.Fail(Messages.AccessDenied);

		var user = _userRepository.GetById(userId, EntityFilter.Removed);
		if (user is null) return ServiceResult<UserDto>.Fail(Messages.UserRestoreFailed);

		_userRepository.Restore(user, updatedByUserId);
		_auditLog.Record(AuditAction.UserRestored, "User", userId, "User restored.");
		return ServiceResult<UserDto>.Ok(user.ToDto(), Messages.UserRestoredSuccessfully);
	}


	internal ServiceResult<User> CreateInitialAdmin(User user, string initialPassword)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (user.Person is null)
			return ServiceResult<User>.Fail("The initial administrator must have Person information.");

		if (string.IsNullOrWhiteSpace(initialPassword))
			throw new ArgumentException("Initial password cannot be empty.", nameof(initialPassword));

		var role = _roleRepository.GetByName(LibraryUserRole.Admin) ??
		           throw new InvalidOperationException("Admin role is missing. Did the migration run?");

		var adminAlreadyExists = _userRepository.GetAll(EntityFilter.Active)
			.Any(existingUser => existingUser.RoleId == role.Id);
		if (adminAlreadyExists) return ServiceResult<User>.Ok(user, "An active administrator already exists.");

		var person = user.Person;
		var duplicate = _uniquenessChecker.Check(person, person.NationalCode, person.Email, person.PhoneNumber);
		if (duplicate != DuplicateField.None)
			return ServiceResult<User>.Fail(
				"The initial administrator's national code, email, or phone number is already in use.");

		user.RoleId = role.Id;
		user.Role = role;

		var passwordResult = _passwordHasher.CreatePasswordHash(initialPassword);
		user.PasswordHash = passwordResult.Hash;
		user.PasswordSalt = passwordResult.Salt;

		_userRepository.Add(user);
		_auditLog.RecordSystemEvent(AuditAction.UserCreated, "User", user.Id,
			"Initial administrator created by the system.");

		return ServiceResult<User>.Ok(user, Messages.UserAddedSuccessfully);
	}


	private static ContributorDto ToContributorDto(UserDto dto)
	{
		return new ContributorDto
		{
			Id = dto.Id,
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			NationalCode = dto.NationalCode,
			Email = dto.Email,
			PhoneNumber = dto.PhoneNumber,
			BirthDate = dto.BirthDate
		};
	}
}