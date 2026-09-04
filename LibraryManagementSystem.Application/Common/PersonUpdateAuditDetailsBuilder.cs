using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Translators;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Common;

public static class PersonUpdateAuditDetailsBuilder
{
	public static string? BuildPersonUpdateAuditDetails(Author author, UpdateAuthorDto dto)
	{
		return BuildPersonUpdateAuditDetails(
			author.FirstName, dto.FirstName,
			author.LastName, dto.LastName,
			author.NationalCode, dto.NationalCode,
			author.Email, dto.Email,
			author.PhoneNumber, dto.PhoneNumber,
			author.BirthDate, dto.BirthDate,
			author.Biography, dto.Biography);
	}


	public static string? BuildPersonUpdateAuditDetails(Translator translator, UpdateTranslatorDto dto)
	{
		return BuildPersonUpdateAuditDetails(
			translator.FirstName, dto.FirstName,
			translator.LastName, dto.LastName,
			translator.NationalCode, dto.NationalCode,
			translator.Email, dto.Email,
			translator.PhoneNumber, dto.PhoneNumber,
			translator.BirthDate, dto.BirthDate);
	}


	private static string? BuildPersonUpdateAuditDetails(
		string firstName, string? newFirstName,
		string lastName, string? newLastName,
		string nationalCode, string? newNationalCode,
		string email, string? newEmail,
		string phoneNumber, string? newPhoneNumber,
		DateOnly birthDate, DateOnly? newBirthDate,
		string? biography = null,
		string? newBiography = null)
	{
		var changes = new List<string>();

		if (newFirstName is not null && newFirstName != firstName)
			changes.Add($"Changed first name from '{firstName}' to '{newFirstName}'.");

		if (newLastName is not null && newLastName != lastName)
			changes.Add($"Changed last name from '{lastName}' to '{newLastName}'.");

		if (newNationalCode is not null && newNationalCode != nationalCode)
			changes.Add($"Changed national code from '{nationalCode}' to '{newNationalCode}'.");

		if (newEmail is not null && newEmail != email) changes.Add($"Changed email from '{email}' to '{newEmail}'.");

		if (newPhoneNumber is not null && newPhoneNumber != phoneNumber)
			changes.Add($"Changed phone number from '{phoneNumber}' to '{newPhoneNumber}'.");

		if (newBirthDate is not null && newBirthDate != birthDate)
			changes.Add(
				$"Changed birth date from '{birthDate:yyyy-MM-dd}' to '{newBirthDate.Value:yyyy-MM-dd}'.");

		if (newBiography is not null && newBiography != biography)
		{
			var oldBio = string.IsNullOrWhiteSpace(biography) ? "None" : biography;
			var newBio = string.IsNullOrWhiteSpace(newBiography) ? "None" : newBiography;
			changes.Add($"Changed biography from '{oldBio}' to '{newBio}'.");
		}

		return changes.Count > 0 ? string.Join(" ", changes) : null;
	}
}