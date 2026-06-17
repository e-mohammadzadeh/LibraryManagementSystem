namespace LibraryManagementSystem.Domain;

public class Loan
{
	public Loan(Book book, LibraryUser user)
	{
		Book = book;
		User = user;
		LoanId = _nextLoanId++;
		BorrowDate = DateOnly.FromDateTime(DateTime.Today);
		DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(DUEDATE));
	}

	public const int DUEDATE = 14;
	private static int _nextLoanId;
	public int LoanId { get; set; }
	public Book Book { get; set; }
	public LibraryUser User { get; set; }
	public DateOnly BorrowDate { get; set; }
	private DateOnly DueDate { get; set; }
	public DateOnly? ReturnDate { get; set; }
	public bool IsOverdue => !ReturnDate.HasValue && DateOnly.FromDateTime(DateTime.Today) > DueDate;
}