using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine
{
	public Fine(Loan loan, string? reason = null)
	{
		ArgumentNullException.ThrowIfNull(loan);
		if (loan.ReturnDate is null) throw new InvalidOperationException("Cannot create a fine for a loan that has not been returned.");
		var overdueDays = loan.ReturnDate.Value.DayNumber - loan.DueDate.DayNumber;
		if (overdueDays <= 0) throw new ArgumentException("Cannot create a fine for a loan that was not returned late.");

		FineId = Guid.CreateVersion7();
		Loan = loan;
		LoanId = loan.LoanId;
		UserId = loan.UserId;
		OverdueDays = overdueDays;
		Amount = FineCalculator(overdueDays);
		Status = FineStatus.Unpaid;
		Reason = reason ?? $"Overdue by {overdueDays} day(s)";
		DailyRate = 1m;
		CreatedAt = DateTime.Now;
	}


	public Guid FineId { get; set; }
	public Guid LoanId { get; set; }
	public Loan Loan { get; set; }
	public Guid UserId { get; set; }
	public int OverdueDays { get; set; }
	public decimal Amount { get; set; }
	public FineStatus Status { get; set; }
	public string Reason { get; set; }
	public decimal DailyRate { get;  set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public DateOnly? PaidAt { get; set; }
}