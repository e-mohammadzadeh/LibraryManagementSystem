using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed record PhoneNumber
{
	public string Value { get; }
	private PhoneNumber(string value) => Value = value;


	public static PhoneNumber Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Phone number cannot be empty.", nameof(value));
		return !Regex.IsMatch(value, @"^\+?[0-9]{7,15}$")
			? throw new ArgumentException("Phone number format is invalid.", nameof(value))
			: new PhoneNumber(value.Trim());
	}


	public override string ToString() => Value;
	public static implicit operator string(PhoneNumber phone) => phone.Value;
}