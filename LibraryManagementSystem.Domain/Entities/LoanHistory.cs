using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class LoanHistory
{
	public LoanHistory(Loan loan, LoanHistoryAction action, string? description = null)
	{
		ArgumentNullException.ThrowIfNull(loan);

		Id = Guid.CreateVersion7();
		Loan = loan;
		LoanId = loan.LoanId;
		User = loan.User;
		UserId = loan.UserId;
		Book = loan.Book;
		BookId = loan.BookId;
		Action = action;
		OccurredAt = DateTime.Now;
		Description = description ?? action.GetDefaultDescription();
		
	}

	public Guid Id { get; private set; }
	public Loan Loan { get; private set; }
	public Guid LoanId { get; private set; }
	public User User { get; private set; }
	public Guid UserId { get; private set; }
	public Book Book { get; private set; }
	public Guid BookId { get; private set; }
	public LoanHistoryAction Action { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public string? Description { get; private set; }
}