using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class AuthUserMapper
{
	public static AuthUserDto ToAuthUserDto(this User user)
	{
		var permissions = user.UserRoles.SelectMany(ur => RolePermissionMap.GetPermissions(ur.Role.Name)).ToHashSet();

		return new AuthUserDto
		{
			Id = user.Id,
			FullName = $"{user.FirstName} {user.LastName}",
			Email = user.Email,
			Roles = [.. user.UserRoles.Select(ur => ur.Role.Name)],
			Permissions = permissions,
			IsActive = user.IsActive,
			MembershipExpiryDate = user.MembershipExpiryDate,
			ShouldRemove = user.ShouldRemove,
			LastLoginDate = user.LastLoginDate
		};
	}
}