using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed record Email
{
	public string Value { get; }
	private Email(string value) => Value = value;


	public static Email Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email cannot be empty.", nameof(value));
		return !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
			? throw new ArgumentException("Email format is invalid.", nameof(value))
			: new Email(value.Trim().ToLowerInvariant());
	}


	public override string ToString() => Value;
	public static implicit operator string(Email email) => email.Value;
}