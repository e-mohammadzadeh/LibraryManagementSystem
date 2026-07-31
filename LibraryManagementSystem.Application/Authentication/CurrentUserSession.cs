using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public class CurrentUserSession : ICurrentUserSession
{
	public bool IsAuthenticated => CurrentUser is not null;
	public int? UserId { get; set; }
	public IReadOnlyList<LibraryUserRole> Roles { get; set; } = [];
	public User? CurrentUser { get; set; }


	public void Login(User user)
	{
		if (user is null) throw new ArgumentNullException(nameof(user));

		CurrentUser = user;
		UserId = user.Id;
		Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList().AsReadOnly();
	}


	public void Logout()
	{
		CurrentUser = null;
		UserId = null;
		Roles = [];
	}

	public bool HasRole(LibraryUserRole role)
	{
		return Roles.Contains(role);
	}
}