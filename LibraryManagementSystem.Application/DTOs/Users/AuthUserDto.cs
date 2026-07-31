using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.Users;

public class AuthUserDto
{
	public int Id { get; init; }
	public string FullName { get; init; } = null!;
	public string Email { get; init; } = null!;
	public IReadOnlyList<LibraryUserRole> Roles { get; init; } = [];
	public bool IsActive { get; init; }
	public DateOnly MembershipExpiryDate { get; init; }
	public bool ShouldRemove { get; init; }
}