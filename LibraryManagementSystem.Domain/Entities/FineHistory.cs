using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class FineHistory
{
	public FineHistory(Fine fine, FineHistoryAction action, string? description = null)
	{
		Id = Guid.CreateVersion7();
		Fine = fine;
		FineId = fine.FineId;
		Loan = fine.Loan;
		LoanId = fine.LoanId;
		User = fine.Loan.User;
		UserId = fine.UserId;
		OverdueDays = fine.OverdueDays;
		Amount = fine.Amount;
		Status = fine.Status;
		Action = action;
		OccurredAt = DateTime.Now;
		Description = description ?? action.GetDefaultDescription();
	}


	public Guid Id { get; private set; }
	public Fine Fine { get; private set; }
	public Guid FineId { get; private set; }
	public Loan Loan { get; private set; }
	public Guid LoanId { get; private set; }
	public User User { get; private set; }
	public Guid UserId { get; private set; }
	public int OverdueDays { get; private set; }
	public decimal Amount { get; private set; }
	public FineStatus Status { get; private set; }
	public FineHistoryAction Action { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public string? Description { get; private set; }
}