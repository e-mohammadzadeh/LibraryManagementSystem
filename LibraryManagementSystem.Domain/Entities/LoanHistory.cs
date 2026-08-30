using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class LoanHistory
{
	public LoanHistory(Loan loan, LoanHistoryAction action, string? description = null)
	{
		Id = ++_nextId;
		LoanId = loan.LoanId;
		UserId = loan.UserId;
		BookId = loan.BookId;
		Action = action;
		OccurredAt = DateTime.Now;
		Description = description ?? action.GetDefaultDescription();
		
	}

	private static int _nextId;
	public int Id { get; private set; }
	public int LoanId { get; private set; }
	public int UserId { get; private set; }
	public int BookId { get; private set; }
	public LoanHistoryAction Action { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public string? Description { get; private set; }
}