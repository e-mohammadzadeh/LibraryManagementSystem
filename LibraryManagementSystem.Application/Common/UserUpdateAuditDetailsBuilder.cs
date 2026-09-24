using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Users;

namespace LibraryManagementSystem.Application.Common;

public static class UserUpdateAuditDetailsBuilder
{
	public static string? BuildUserUpdateAuditDetails(User user, UpdateUserDto dto, Role resolvedRole)
	{
		var changes = new List<string>();

		if (dto.FirstName is not null && dto.FirstName != user.FirstName)
			changes.Add($"Changed first name from '{user.FirstName}' to '{dto.FirstName}'.");

		if (dto.LastName is not null && dto.LastName != user.LastName)
			changes.Add($"Changed last name from '{user.LastName}' to '{dto.LastName}'.");

		if (dto.NationalCode is not null && dto.NationalCode != user.NationalCode)
			changes.Add($"Changed national code from '{user.NationalCode}' to '{dto.NationalCode}'.");

		if (dto.Email is not null && dto.Email != user.Email)
			changes.Add($"Changed email from '{user.Email}' to '{dto.Email}'.");

		if (dto.PhoneNumber is not null && dto.PhoneNumber != user.PhoneNumber)
			changes.Add($"Changed phone number from '{user.PhoneNumber}' to '{dto.PhoneNumber}'.");

		if (dto.BirthDate is not null && dto.BirthDate != user.BirthDate)
			changes.Add($"Changed birth date from '{user.BirthDate:yyyy-MM-dd}' to '{dto.BirthDate.Value:yyyy-MM-dd}'.");

		if (user.Role.Name != resolvedRole.Name)
			changes.Add($"Changed roles from '{user.Role.Name}' to '{resolvedRole.Name}'.");

		return changes.Count > 0 ? string.Join(" ", changes) : null;
	}
}