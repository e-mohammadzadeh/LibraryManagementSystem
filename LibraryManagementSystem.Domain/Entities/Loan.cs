using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Loan
{
	public Loan(Book book, User user)
	{
		LoanId = ++_nextLoanId;
		Book = book;
		User = user;
		BorrowDate = DateOnly.FromDateTime(DateTime.Today);
		DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(DUEDATE));
	}

	public const int DUEDATE = 14;
	private static int _nextLoanId;
	public int LoanId { get; set; }
	public Book Book { get; set; }
	public int BookId { get; set; }
	public User User { get; set; }
	public int UserId { get; set; }
	public DateOnly BorrowDate { get; set; }
	private DateOnly DueDate { get; set; }
	public DateOnly? ReturnDate { get; set; }
	public LoanStatus Status { get; set; }
	public bool IsOverdue => !ReturnDate.HasValue && DateOnly.FromDateTime(DateTime.Today) > DueDate;
}