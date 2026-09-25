namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed record ISBN
{
	public string Value { get; }

	private ISBN(string value) => Value = value;


	public static ISBN Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ISBN cannot be empty.", nameof(value));

		var normalized = value.Replace("-", "").Replace(" ", "").ToUpperInvariant();

		if (normalized.Length != 10 && normalized.Length != 13)
			throw new ArgumentException("ISBN must be 10 or 13 characters long.", nameof(value));

		var isValid = normalized.Length == 10
			? IsValidIsbn10(normalized)
			: IsValidIsbn13(normalized);

		return !isValid
			? throw new ArgumentException("ISBN checksum is invalid.", nameof(value))
			: new ISBN(normalized);
	}


	private static bool IsValidIsbn10(string value)
	{
		var sum = 0;
		for (var i = 0; i < 10; i++)
		{
			var c = value[i];
			int digit;
			if (c == 'X' && i == 9)
				digit = 10;
			else if (char.IsDigit(c))
				digit = c - '0';
			else
				return false;

			sum += digit * (10 - i);
		}

		return sum % 11 == 0;
	}


	private static bool IsValidIsbn13(string value)
	{
		var sum = 0;
		for (var i = 0; i < 13; i++)
		{
			if (!char.IsDigit(value[i])) return false;
			var digit = value[i] - '0';
			sum += i % 2 == 0 ? digit : digit * 3;
		}

		return sum % 10 == 0;
	}


	public override string ToString() => Value;

	public static implicit operator string(ISBN isbn) => isbn.Value;
}