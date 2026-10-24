using LibraryManagementSystem.Infrastructure.DTOs.Users;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Authentication;

public class CurrentUserSession : ICurrentUserSession
{
	public AuthUserDto CurrentUser { get; set; } = null!;
	public bool IsAuthenticated { get; private set; }
	public IReadOnlySet<Permission> Permissions => CurrentUser.Permissions;


	public void Login(AuthUserDto user)
	{
		CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
		IsAuthenticated = true;
	}


	public void Logout()
	{
		IsAuthenticated = false;
		CurrentUser = null!;
	}


	public void UpdateCurrentUser(AuthUserDto user)
	{
		ArgumentNullException.ThrowIfNull(user);
		if (!IsAuthenticated) throw new InvalidOperationException("No user is currently logged in.");
		CurrentUser = user;
	}


	private bool HasRole(LibraryUserRole role) { return CurrentUser.RoleName.Equals(role); }

	public bool IsAdmin => HasRole(LibraryUserRole.Admin);
	public bool IsLibrarian => HasRole(LibraryUserRole.Librarian);
	public bool IsMember => HasRole(LibraryUserRole.Member);


	public bool IsSelfServiceMember => IsMember && !IsAdmin && !IsLibrarian;
}