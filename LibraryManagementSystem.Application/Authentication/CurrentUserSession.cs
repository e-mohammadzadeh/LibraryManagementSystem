using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public class CurrentUserSession : ICurrentUserSession
{
	public AuthUserDto? CurrentUser { get; private set; }
	public Guid? UserId => CurrentUser?.Id;
	public bool IsAuthenticated => CurrentUser is not null;
	public IReadOnlySet<Permission> Permissions => CurrentUser?.Permissions ?? new HashSet<Permission>();


	public void Login(AuthUserDto user) { CurrentUser = user ?? throw new ArgumentNullException(nameof(user)); }
	public void Logout() => CurrentUser = null;
	public void UpdateCurrentUser(AuthUserDto user)
	{
		CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
	}


	private bool HasRole(LibraryUserRole role) { return CurrentUser?.Role.Equals(role) ?? false; }

	public bool IsAdmin => HasRole(LibraryUserRole.Admin);
	public bool IsLibrarian => HasRole(LibraryUserRole.Librarian);
	public bool IsMember => HasRole(LibraryUserRole.Member);


	public bool IsSelfServiceMember => IsMember && !IsAdmin && !IsLibrarian;
}