using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public class CurrentUserSession : ICurrentUserSession
{
	public AuthUserDto? CurrentUser { get; private set; }
	public bool IsAuthenticated => CurrentUser is not null;
	public int? UserId => CurrentUser?.Id;

	public void Login(AuthUserDto user) { CurrentUser = user ?? throw new ArgumentNullException(nameof(user)); }

	public void Logout() => CurrentUser = null;

	public bool HasRole(LibraryUserRole role) { return CurrentUser?.Roles.Contains(role) ?? false; }
}