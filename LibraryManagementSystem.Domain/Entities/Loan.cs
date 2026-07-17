using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Loan
{
	public Loan(Book book, User user)
	{
		LoanId = ++_nextLoanId;
		Book = book;
		BookId = book.BookId;
		User = user;
		UserId = user.Id;
		BorrowDate = DateOnly.FromDateTime(DateTime.Today);
		DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(LoanPeriodDays));
		ReturnDate = null;
		Status = LoanStatus.Borrowed;
	}


	private const int LoanPeriodDays = 14;
	public const int MaxRenewals = 1;
	private static int _nextLoanId;
	public int LoanId { get; private set; }
	public Book Book { get; private set; }
	public int BookId { get; private set; }
	public User User { get; private set; }
	public int UserId { get; private set; }
	public DateOnly BorrowDate { get; private set; }
	public DateOnly DueDate { get; private set; }
	public DateOnly? ReturnDate { get; private set; }
	public LoanStatus Status { get; private set; }
	public int RenewalCount { get; private set; }
	public bool IsOverdue => !ReturnDate.HasValue && DateOnly.FromDateTime(DateTime.Today) > DueDate;


	public void MarkAsReturned()
	{
		ReturnDate = DateOnly.FromDateTime(DateTime.Today);
		Status = LoanStatus.Returned;
	}


	public void Renew()
	{
		if (ReturnDate != null)
			throw new InvalidOperationException("Returned books cannot be renewed.");

		if (RenewalCount >= MaxRenewals)
			throw new InvalidOperationException("This loan has already reached the maximum number of renewals.");

		DueDate = DueDate.AddDays(14);
		Status = LoanStatus.Borrowed;
		RenewalCount++;
	}
}