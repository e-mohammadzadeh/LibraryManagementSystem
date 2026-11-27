using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

namespace LibraryManagementSystem.Application.Mapping;

public static class UserMapper
{
	public static UserDto ToDto(this User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var person = user.Person ??
		             throw new InvalidOperationException("User.Person was not loaded. Include(Person) is required.");


		return new UserDto
		{
			Id = user.Id,
			PersonId = user.PersonId,
			FirstName = person.FirstName,
			LastName = person.LastName,
			NationalCode = person.NationalCode,
			Email = person.Email,
			PhoneNumber = person.PhoneNumber,
			BirthDate = person.BirthDate,
			RoleName = user.Role.Name,
			MembershipStartDate = user.MembershipStartDate,
			MembershipExpiryDate = user.MembershipExpiryDate,
			ShouldRemove = user.ShouldRemove,
			IsActive = user.IsActive,
			IsRemoved = user.IsRemoved,
			CreatedAt = user.CreatedAt,
			UpdatedAt = user.UpdatedAt,
			LastLoginDate = user.LastLoginDate
		};
	}
}