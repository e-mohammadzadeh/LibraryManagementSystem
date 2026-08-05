using LibraryManagementSystem.Application.DTOs.Users;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class UserMapper
{
	public static UserDto ToDto(this User user)
	{
		return new UserDto
		{
			Id = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			NationalCode = user.NationalCode,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,
			BirthDate = user.BirthDate,
			Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList().AsReadOnly(),
			MembershipStartDate = user.MembershipStartDate,
			MembershipExpiryDate = user.MembershipExpiryDate,
			IsActive = user.IsActive,
			CreatedAt = user.CreatedAt,
			UpdatedAt = user.UpdatedAt
		};
	}
}