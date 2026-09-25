namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed record Money
{
	public decimal Amount { get; }
	private Money(decimal amount) => Amount = amount;


	public static Money Create(decimal amount)
	{
		if (amount < 0)
			throw new ArgumentException("Money amount cannot be negative.", nameof(amount));
		return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
	}

	public static Money Zero => new(0m);
	public Money Add(Money other) => Create(Amount + other.Amount);
	public Money Subtract(Money other) => Create(Amount - other.Amount);
	public static Money operator +(Money left, Money right) => left.Add(right);
	public static Money operator -(Money left, Money right) => left.Subtract(right);
	public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
	public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
	public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;
	public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;

	public override string ToString() => Amount.ToString("C");

	public static implicit operator decimal(Money money) => money.Amount;

}