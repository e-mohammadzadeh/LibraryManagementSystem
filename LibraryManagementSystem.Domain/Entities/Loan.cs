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
	public bool IsActive => ReturnDate is not null;


	public void MarkAsReturned()
	{
		ReturnDate = DateOnly.FromDateTime(DateTime.Today);
		Status = LoanStatus.Returned;
	}


	public bool CanRenew(out string errorMessage)
	{
		if (ReturnDate != null)
		{
			errorMessage = "Returned books cannot be renewed.";
			return false;
		}

		if (RenewalCount >= MaxRenewals)
		{
			errorMessage = "This loan has already reached the maximum number of renewals.";
			return false;
		}

		// checking IsOverdue is true of false 

		errorMessage = string.Empty;
		return true;
	}


	public void Renew()
	{
		if (!CanRenew(out var errorMessage))
			throw new InvalidOperationException(errorMessage);
		
		DueDate = DueDate.AddDays(LoanPeriodDays);
		Status = LoanStatus.Borrowed;
		RenewalCount++;
	}
}