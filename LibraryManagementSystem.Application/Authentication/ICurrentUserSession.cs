using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authentication;

public interface ICurrentUserSession
{
	bool IsAuthenticated { get; set; }
	int? UserId { get; set; }
	IReadOnlyList<LibraryUserRole> Roles { get; set; }
	User? CurrentUser { get; set; }

	void Login(User user);
	void Logout();
	bool HasRole(string roleName);
}