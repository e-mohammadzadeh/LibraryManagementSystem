using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

namespace LibraryManagementSystem.Application.Mapping;

public static class AuthUserMapper
{
	public static AuthUserDto ToAuthUserDto(this User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		var person = user.Person ?? throw new InvalidOperationException("User.Person was not loaded. Include(Person) is required.");

		return new AuthUserDto
		{
			Id = user.Id,
			FullName = $"{person.FirstName} {person.LastName}",
			Email = person.Email,
			RoleName = user.Role.Name,
			Permissions = RolePermissionMap.GetPermissions(user.Role.Name).ToHashSet(),
			IsActive = user.IsActive,
			MembershipExpiryDate = user.MembershipExpiryDate,
			ShouldRemove = user.ShouldRemove,
			LastLoginDate = user.LastLoginDate
		};
	}
}