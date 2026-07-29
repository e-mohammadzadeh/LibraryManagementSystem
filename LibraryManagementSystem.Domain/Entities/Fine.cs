using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine
{
	public Fine(Loan loan, int overdueDays, string? reason = null)
	{
		if (loan is null) throw new ArgumentNullException(nameof(loan));
		if (overdueDays <= 0) throw new ArgumentException("Overdue days must be greater than zero.");

		FineId = ++_nextFineId;
		Loan = loan;
		LoanId = loan.LoanId;
		UserId = loan.UserId;
		OverdueDays = overdueDays;
		Amount = FineCalculator(overdueDays);
		Status = FineStatus.Unpaid;
		Reason = reason ?? $"Overdue by {overdueDays} day(s)";
		CreatedAt = DateTime.Now;
	}


	private static int _nextFineId;
	public int FineId { get; private set; }
	public int LoanId { get; set; }
	public Loan Loan { get; set; }
	public int UserId { get; set; }
	public int OverdueDays { get; private set; }
	public decimal Amount { get; private set; }
	public FineStatus Status { get; private set; }
	public string Reason { get; set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime? UpdatedAt { get; set; }
	public DateOnly? PaidAt { get; set; }


	public static decimal FineCalculator(int overdueDays)
	{
		const decimal maxUnpaidFineThreshold = 150m; // 15 days hits cap
		const decimal initialDailyRate = 1m;
		const int fixedRateDays = 3;
		const decimal geometricRatio = 1.5m;

		if (overdueDays <= 0) return 0m;

		var flatTotal = Math.Min(overdueDays, fixedRateDays) * initialDailyRate;

		if (overdueDays <= fixedRateDays) return Math.Min(flatTotal, maxUnpaidFineThreshold);

		var geometricDays = overdueDays - fixedRateDays;
		var geometricTotal = initialDailyRate * ((decimal)Math.Pow((double)geometricRatio, geometricDays) - 1m) /
			(geometricRatio - 1m);

		var total = flatTotal + geometricTotal;
		return Math.Min(total, maxUnpaidFineThreshold);
	}


	public void Pay()
	{
		if (Status == FineStatus.Paid) throw new InvalidOperationException("Fine is already paid.");
		if (Status == FineStatus.Waived) throw new InvalidOperationException("Fine has been waived.");
		Status = FineStatus.Paid;
		PaidAt = DateOnly.FromDateTime(DateTime.Today);
		UpdatedAt = DateTime.Now;
	}


	public void Waive()
	{
		if (Status == FineStatus.Paid) throw new InvalidOperationException("Cannot waive an already paid fine.");
		Status = FineStatus.Waived;
		UpdatedAt = DateTime.Now;
	}
}