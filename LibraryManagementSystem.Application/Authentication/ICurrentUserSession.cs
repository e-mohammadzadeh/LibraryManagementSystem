using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

namespace LibraryManagementSystem.Application.Authentication;

public interface ICurrentUserSession
{
	AuthUserDto? CurrentUser { get; }
	bool IsAuthenticated { get; }
	Guid? UserId { get; }

	void Login(AuthUserDto user);
	void Logout();
	void UpdateCurrentUser(AuthUserDto user);
	IReadOnlySet<Permission> Permissions { get; }
	bool IsAdmin { get; }
	bool IsLibrarian { get; }
	bool IsMember { get; }
	bool IsSelfServiceMember { get; }
}