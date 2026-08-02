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


	public bool HasAnyRole(params LibraryUserRole[]? roles)
	{
		if (CurrentUser is null || roles is null || roles.Length == 0) return false;

		return roles.Any(r => CurrentUser.Roles.Contains(r));
	}


	public bool IsAdmin => HasRole(LibraryUserRole.Admin);
	public bool IsLibrarian => HasRole(LibraryUserRole.Librarian);
	public bool IsMember => HasRole(LibraryUserRole.Member);


	public bool CanBorrowBooks =>
		IsAuthenticated && 
		CurrentUser!.IsActive && 
		!CurrentUser.ShouldRemove &&
		CurrentUser.MembershipExpiryDate >= DateOnly.FromDateTime(DateTime.Today);
}