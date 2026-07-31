using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;
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
}