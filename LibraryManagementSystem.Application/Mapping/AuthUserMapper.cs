using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

namespace LibraryManagementSystem.Application.Mapping;

public static class AuthUserMapper
{
	public static AuthUserDto ToAuthUserDto(this User user)
	{
		var permissions = user.Role.SelectMany(ur => RolePermissionMap.GetPermissions(ur.Role.Name)).ToHashSet();

		return new AuthUserDto
		{
			Id = user.Id,
			FullName = $"{user.FirstName} {user.LastName}",
			Email = user.Email,
			Role = user.Role.Name,
			Permissions = permissions,
			IsActive = user.IsActive,
			MembershipExpiryDate = user.MembershipExpiryDate,
			ShouldRemove = user.ShouldRemove,
			LastLoginDate = user.LastLoginDate
		};
	}
}