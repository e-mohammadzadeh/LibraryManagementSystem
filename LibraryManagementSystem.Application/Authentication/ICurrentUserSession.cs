using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public interface ICurrentUserSession
{
	AuthUserDto? CurrentUser { get; }
	bool IsAuthenticated { get; }
	int? UserId { get; }

	void Login(AuthUserDto user);
	void Logout();
	void UpdateCurrentUser(AuthUserDto user);
	IReadOnlySet<Permission> Permissions { get; }
	bool IsAdmin { get; }
	bool IsLibrarian { get; }
	bool IsMember { get; }
	bool IsSelfServiceMember { get; }
}