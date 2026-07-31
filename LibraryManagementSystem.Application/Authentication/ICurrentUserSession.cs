using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public interface ICurrentUserSession
{
	bool IsAuthenticated { get; }
	int? UserId { get; }
	IReadOnlyList<LibraryUserRole> Roles { get; }
	User? CurrentUser { get; }

	void Login(User user);
	void Logout();
	bool HasRole(LibraryUserRole role);
}