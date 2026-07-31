using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public class CurrentUserSession : ICurrentUserSession
{
	public bool IsAuthenticated => CurrentUser is not null;
	public int? UserId { get; private set; }
	public IReadOnlyList<LibraryUserRole> Roles { get; private set; } = [];
	public User? CurrentUser { get; private set; }

	public void Login(User user) { CurrentUser = user; }
	public void Logout() { CurrentUser = null; }

	public static bool HasRole(string roleName)
	{
		return CurrentUser?.Roles.Contains(roleName) ?? false;
	}
}