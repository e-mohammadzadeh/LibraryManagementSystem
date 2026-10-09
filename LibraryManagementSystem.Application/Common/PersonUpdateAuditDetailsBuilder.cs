using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Common;

namespace LibraryManagementSystem.Application.Common;

public static class PersonUpdateAuditDetailsBuilder
{
	public static string? BuildPersonUpdateAuditDetails(Contributor contributor, ContributorDto dto)
	{
		var changes = new List<string>();

		AddIfChanged(changes, "first name", contributor.FirstName, dto.FirstName);
		AddIfChanged(changes, "last name", contributor.LastName, dto.LastName);
		AddIfChanged(changes, "national code", contributor.NationalCode, dto.NationalCode, sensitive: true);
		AddIfChanged(changes, "email", contributor.Email.Value, dto.Email!, sensitive: true);
		AddIfChanged(changes, "phone number", contributor.PhoneNumber.Value, dto.PhoneNumber!, sensitive: true);
		AddIfChanged(changes, "birth date", contributor.BirthDate.ToString(ValidationConstants.DateFormat),
			dto.BirthDate?.ToString(ValidationConstants.DateFormat));
		AddIfChanged(changes, "biography", contributor.Biography, dto.Biography);

		return changes.Count > 0 ? string.Join(" ", changes) : null;
	}


	private static void AddIfChanged(List<string> changes, string field, string? oldValue, string? newValue,
		bool sensitive = false)
	{
		// A null new value means "this field is not being updated".
		if (newValue is null) return;

		var oldText = string.IsNullOrWhiteSpace(oldValue) ? "None" : oldValue;
		var newText = string.IsNullOrWhiteSpace(newValue) ? "None" : newValue;

		if (oldText == newText) return;

		changes.Add(sensitive
			? $"Changed {field}."
			: $"Changed {field} from '{oldText}' to '{newText}'.");
	}
}