using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine
{
	public Fine(int loanId, Loan loan, int userId, decimal amount, DateOnly issuedDate, FineStatus status, DateOnly paidDate, string reason)
	{
		FineId = ++_nextFineId;
		LoanId = loanId;
		Loan = loan;
		UserId = userId;
		Amount = amount;
		IssuedDate = issuedDate;
		Status = status;
		PaidAt = paidDate;
		Reason = reason;
	}


	private static int _nextFineId;
	public int FineId { get; set; }
	public int LoanId { get; set; }
	public Loan Loan { get; set; }
	public int UserId { get; set; }
	public decimal Amount { get; set; }
	public DateOnly IssuedDate { get; set; }
	public FineStatus Status { get; set; }
	public DateOnly CreatedAt { get; set; }
	public DateOnly PaidAt { get; set; }
	public string Reason { get; set; }
}