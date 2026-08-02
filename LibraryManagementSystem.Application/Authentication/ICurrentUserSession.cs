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
	bool HasRole(LibraryUserRole role);
	bool HasAnyRole(params LibraryUserRole[] roles);
	bool IsAdmin { get; }
	bool IsLibrarian { get; }
	bool IsMember { get; }
	bool CanBorrowBooks { get; }
}