using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class LoanHistory
{
	public LoanHistory(Loan loan, LoanHistoryAction action, string? description = null)
	{
		ArgumentNullException.ThrowIfNull(loan);

		Id = ++_nextId;
		Loan = loan;
		LoanId = loan.LoanId;
		User = loan.User;
		UserId = loan.UserId;
		Book = loan.Book;
		BookId = loan.BookId;
		Action = action;
		OccurredAt = DateTime.Now;
		Description = description ?? LoanHistoryActionExtensions.GetDefaultDescription();
		
	}

	private static int _nextId;
	public int Id { get; private set; }
	public Loan Loan { get; private set; } = null!;
	public int LoanId { get; private set; }
	public User User { get; private set; } = null!;
	public int UserId { get; private set; }
	public Book Book { get; private set; } = null!;
	public int BookId { get; private set; }
	public LoanHistoryAction Action { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public string? Description { get; private set; }
}